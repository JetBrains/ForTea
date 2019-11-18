using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.OutsideSolution;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules {

	/// <summary>
	/// Manages <see cref="IT4FilePsiModule"/> for T4 files.
	/// Contains common implementation for <see cref="T4ProjectPsiModuleHandler"/>
	/// and <see cref="T4MiscFilesProjectPsiModuleProvider"/>.
	/// </summary>
	internal sealed class T4PsiModuleProvider : IDisposable {
		[NotNull] private readonly Dictionary<IProjectFile, ModuleWrapper> _modules = new Dictionary<IProjectFile, ModuleWrapper>();
		private readonly Lifetime _lifetime;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IT4Environment _t4Environment;

		[NotNull]
		private IT4TemplateKindProvider TemplateDataManager { get; }

		private readonly struct ModuleWrapper {

			[NotNull] public readonly T4FilePsiModule Module;
			[NotNull] public readonly LifetimeDefinition LifetimeDefinition;

			public ModuleWrapper([NotNull] T4FilePsiModule module, [NotNull] LifetimeDefinition lifetimeDefinition) {
				Module = module;
				LifetimeDefinition = lifetimeDefinition;
			}
		}

		/// <summary>Gets all <see cref="IT4FilePsiModule"/>s for opened files.</summary>
		/// <returns>A collection of <see cref="IT4FilePsiModule"/>.</returns>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<IPsiModule> GetModules() {
			_shellLocks.AssertReadAccessAllowed();

			return _modules.Values.Select(wrapper => (IPsiModule) wrapper.Module);
		}

		/// <summary>Gets all source files for a given project file.</summary>
		/// <param name="projectFile">The project file whose source files will be returned.</param>
		[NotNull]
		[ItemNotNull]
		public IList<IPsiSourceFile> GetPsiSourceFilesFor([CanBeNull] IProjectFile projectFile) {
			_shellLocks.AssertReadAccessAllowed();

			return projectFile?.IsValid() == true
				&& _modules.TryGetValue(projectFile, out ModuleWrapper wrapper)
				&& wrapper.Module.IsValid()
			? new[] { wrapper.Module.SourceFile }
			: EmptyList<IPsiSourceFile>.InstanceList;
		}

		/// <summary>
		/// Processes changes for specific project file and sets up a list of corresponding source file changes.
		/// </summary>
		/// <param name="projectFile">The project file.</param>
		/// <param name="changeType">Type of the change.</param>
		/// <param name="changeBuilder">The change builder used to populate changes.</param>
		/// <returns><see cref="PsiModuleChange.ChangeType"/> if further changing is required, null otherwise</returns>
		public PsiModuleChange.ChangeType? OnProjectFileChanged(
			[NotNull] IProjectFile projectFile,
			PsiModuleChange.ChangeType changeType,
			[NotNull] PsiModuleChangeBuilder changeBuilder
		) {
			if (!_t4Environment.IsSupported) return changeType;
			_shellLocks.AssertWriteAccessAllowed();
			ModuleWrapper moduleWrapper;
			switch (changeType)
			{
				case PsiModuleChange.ChangeType.Added:
					// Preprocessed .tt files should be handled by R# itself as if it's a normal project file,
					// so that it has access to the current project types.
					if (projectFile.LanguageType.Is<T4ProjectFileType>()
					    && !TemplateDataManager.IsPreprocessedTemplate(projectFile))
					{
						AddFile(projectFile, changeBuilder);
						return null;
					}
					break;

				case PsiModuleChange.ChangeType.Removed:
					if (_modules.TryGetValue(projectFile, out moduleWrapper)) {
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
						return null;
					}
					break;

				case PsiModuleChange.ChangeType.Modified:
					if (_modules.TryGetValue(projectFile, out moduleWrapper)) {
						if (!TemplateDataManager.IsPreprocessedTemplate(projectFile)) {
							ModifyFile(changeBuilder, moduleWrapper);
							return null;
						}

						// The T4 file went from Transformed to Preprocessed, it doesn't need a T4PsiModule anymore.
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
						return PsiModuleChange.ChangeType.Added;
					}

					// The T4 file went from Preprocessed to Transformed, it now needs a T4PsiModule.
					if (projectFile.LanguageType.Is<T4ProjectFileType>()
					    && !TemplateDataManager.IsPreprocessedTemplate(projectFile))
					{
						AddFile(projectFile, changeBuilder);
						return PsiModuleChange.ChangeType.Removed;
					}

					break;

			}

			return changeType;
		}

		private void AddFile([NotNull] IProjectFile projectFile, [NotNull] PsiModuleChangeBuilder changeBuilder) {
			ISolution solution = projectFile.GetSolution();

			// creates a new T4PsiModule for the file
			LifetimeDefinition lifetimeDefinition = Lifetime.Define(_lifetime, "[T4]" + projectFile.Name);
			var psiModule = new T4FilePsiModule(
				lifetimeDefinition.Lifetime,
				projectFile,
				_changeManager,
				_shellLocks,
				_t4Environment
			);
			_modules[projectFile] = new ModuleWrapper(psiModule, lifetimeDefinition);
			changeBuilder.AddModuleChange(psiModule, PsiModuleChange.ChangeType.Added);
			changeBuilder.AddFileChange(psiModule.SourceFile, PsiModuleChange.ChangeType.Added);

			// Invalidate files that had this specific files as an include,
			// and whose IPsiSourceFile was previously managed by T4OutsideSolutionSourceFileManager.
			// Those files will be reparsed with the new source file.
			var fileManager = solution.GetComponent<T4OutsideSolutionSourceFileManager>();
			FileSystemPath location = projectFile.Location;
			if (fileManager.HasSourceFile(location)) {
				fileManager.DeleteSourceFile(location);
			}
		}

		private void RemoveFile([NotNull] IProjectFile projectFile, [NotNull] PsiModuleChangeBuilder changeBuilder, ModuleWrapper moduleWrapper) {
			_modules.Remove(projectFile);
			changeBuilder.AddFileChange(moduleWrapper.Module.SourceFile, PsiModuleChange.ChangeType.Removed);
			changeBuilder.AddModuleChange(moduleWrapper.Module, PsiModuleChange.ChangeType.Removed);
			moduleWrapper.LifetimeDefinition.Terminate();
		}

		private static void ModifyFile([NotNull] PsiModuleChangeBuilder changeBuilder, ModuleWrapper moduleWrapper)
			=> changeBuilder.AddFileChange(moduleWrapper.Module.SourceFile, PsiModuleChange.ChangeType.Modified);

		public void Dispose() {
			using (WriteLockCookie.Create()) {
				foreach (var wrapper in _modules.Values)
					wrapper.LifetimeDefinition.Terminate();
				_modules.Clear();
			}
		}

		internal T4PsiModuleProvider(
			Lifetime lifetime,
			[NotNull] IShellLocks shellLocks,
			[NotNull] ChangeManager changeManager,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IT4TemplateKindProvider templateDataManager
		)
		{
			_lifetime = lifetime;
			_shellLocks = shellLocks;
			_changeManager = changeManager;
			_t4Environment = t4Environment;
			TemplateDataManager = templateDataManager;
		}

	}

}
