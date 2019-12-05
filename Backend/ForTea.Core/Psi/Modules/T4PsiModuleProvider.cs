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
		)
		{
			if (!_t4Environment.IsSupported)
				// The plugin does not operate in
				// old versions of Visual Studio
				// and unknown environments.
				return changeType;
			// It would be logical to check the file type here and return if it's not T4.
			// However, this is impossible because calculating file type
			// requires the file to be attached to a project hierarchy,
			// which sometimes doesn't hold.
			_shellLocks.AssertWriteAccessAllowed();
			ModuleWrapper moduleWrapper;
			switch (changeType)
			{
				case PsiModuleChange.ChangeType.Added:
					if (!projectFile.LanguageType.Is<T4ProjectFileType>())
						// We only handle T4 files and do not affect other project files.
						break;
					if (TemplateDataManager.IsPreprocessedTemplate(projectFile))
						// Otherwise, this is a new preprocessed file.
						// We don't create modules for preprocessed files
						// so that to let R# add them into the project,
						// so that the template will have access to the current project types.
						break;
					// This is a new executable file, so we need to create a module for it.
					AddFile(projectFile, changeBuilder);
					// After the module is created, the request to add the file has been handled,
					// so there's no need to create any other modules, so pass null as RequestedChange.
					return null;

				case PsiModuleChange.ChangeType.Removed:
					if (!_modules.TryGetValue(projectFile, out moduleWrapper))
						// The file wasn't handled by us in the first place,
						// so there's no way we can handle its removal.
						break;
					RemoveFile(projectFile, changeBuilder, moduleWrapper);
					// Since we handled the module removal, there's nothing else to be done.
					return null;

				case PsiModuleChange.ChangeType.Modified:
					if (_modules.TryGetValue(projectFile, out moduleWrapper))
					{
						if (TemplateDataManager.IsPreprocessedTemplate(projectFile))
						{
							// The T4 file has a module but it shouldn't because it's preprocessed.
							// This can happen when an executable file becomes preprocessed.
							// We no longer need the module for it.
							RemoveFile(projectFile, changeBuilder, moduleWrapper);
							// After the module has been removed,
							// the file needs to be added to the project it resides in.
							// Requesting this change does exactly that.
							return PsiModuleChange.ChangeType.Added;
						}

						// This is the ordinary change in an executable T4 file.
						// We can handle it.
						ModifyFile(changeBuilder, moduleWrapper);
						// Since we've handled the change, no need to delegate it to R#.
						return null;
					}

					// We don't know about this file. Maybe it is a T4 file we should become interested in?
					if (!projectFile.LanguageType.Is<T4ProjectFileType>())
						// No it's not.
						break;
					if (TemplateDataManager.IsPreprocessedTemplate(projectFile))
						// It is still a preprocessed file, we still don't want a module for it.
						// Let R# continue managing this file.
						break;
					// The T4 is executable but has no module.
					// This can happen if it used to be preprocessed but became executable.
					// It now needs a T4PsiModule.
					AddFile(projectFile, changeBuilder);
					// After we've created this module,
					// we need to let R# know that this file is now our business.
					// That's why we request this file to be removed from the project it resides in.
					return PsiModuleChange.ChangeType.Removed;
			}

			// If there's nothing we can do about this file, let R# work.
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
