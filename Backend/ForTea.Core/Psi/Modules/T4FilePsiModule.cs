using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.Modules.References;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Web.Impl.PsiModules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	/// <summary>PSI module managing a single T4 file.</summary>
	public sealed class T4FilePsiModule : ProjectPsiModuleBase, IT4FilePsiModule
	{
		private readonly Lifetime _lifetime;
		[NotNull] private readonly T4AssemblyReferenceManager _assemblyReferenceManager;
		[NotNull] private readonly T4ProjectReferenceManager _projectReferenceManager;
		[NotNull] private readonly IPsiModules _psiModules;
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly IT4Environment _t4Environment;

		[NotNull]
		private T4AssemblyReferenceInvalidator AssemblyReferenceInvalidator { get; }

		[NotNull]
		private IProjectFile File { get; }

		private IChangeProvider ChangeProvider { get; } = new FakeChangeProvider();

		/// <summary>
		/// Gets the language used by this PSI module.
		/// This should be the code behind language, not the primary language.
		/// </summary>
		public override PsiLanguageType PsiLanguage => T4Language.Instance;

		/// <summary>
		/// Gets the project file type used by this PSI module:
		/// always <see cref="JetBrains.ProjectModel.ProjectFileType"/>.
		/// </summary>
		public override ProjectFileType ProjectFileType => T4ProjectFileType.Instance;

		/// <summary>Returns the source file associated with this PSI module.</summary>
		public IPsiSourceFile SourceFile { get; }

		[NotNull, ItemNotNull]
		protected override IEnumerable<IPsiSourceFile> GetSourceFiles() => new[] {SourceFile};

		private void OnFileDataChanged(Pair<IPsiSourceFile, T4DeclaredAssembliesDiff> pair)
		{
			(IPsiSourceFile first, T4DeclaredAssembliesDiff second) = pair;
			if (first != SourceFile) return;
			if (_shellLocks.IsWriteAccessAllowed())
				OnFileDataChanged(second);
			else
			{
				_shellLocks.ExecuteOrQueueEx(_lifetime, "T4PsiModuleOnFileDataChanged",
					() => _shellLocks.ExecuteWithWriteLock(() => OnFileDataChanged(second)));
			}
		}

		/// <summary>Called when the associated data file changed: added/removed assemblies or includes.</summary>
		/// <param name="dataDiff">The difference between the old and new data.</param>
		private void OnFileDataChanged([NotNull] T4DeclaredAssembliesDiff dataDiff)
		{
			_shellLocks.AssertWriteAccessAllowed();
			bool hasChanges = AssemblyReferenceInvalidator.InvalidateAssemblies(
				dataDiff,
				File,
				_assemblyReferenceManager
			);

			if (!hasChanges) return;

			// tells the world the module has changed
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.Modified);
			// TODO: mark as dirty
			_shellLocks.ExecuteOrQueueEx("T4PsiModuleChange",
				() => _changeManager.ExecuteAfterChange(
					() => _shellLocks.ExecuteWithWriteLock(
						() => _changeManager.OnProviderChanged(ChangeProvider, changeBuilder.Result,
							SimpleTaskExecutor.Instance)
					)
				)
			);
		}

		protected override IEnumerable<IPsiModuleReference> GetReferencesInternal()
		{
			var references = new PsiModuleReferenceAccumulator(TargetFrameworkId);
			var moduleReferences = RawReferences
				.SelectNotNull(assembly => _psiModules.GetPrimaryPsiModule(assembly, TargetFrameworkId))
				.Select(it => new PsiModuleReference(it));
			references.AddRange(moduleReferences);
			references.AddRange(_projectReferenceManager.GetProjectReference());
			return references.GetReferences();
		}

		public IEnumerable<IAssembly> RawReferences => _assemblyReferenceManager
			.References
			.Values
			.SelectNotNull(cookie => cookie.Assembly);

		[NotNull]
		private PsiProjectFile CreateSourceFile([NotNull] IProjectFile projectFile,
			[NotNull] DocumentManager documentManager)
			=> new PsiProjectFile(
				this,
				projectFile,
				(pf, sf) => new T4PsiProjectFileProperties(pf, sf, true),
				JetFunc<IProjectFile, IPsiSourceFile>.True,
				documentManager,
				_assemblyReferenceManager.ResolveContext
			);

		/// <summary>Disposes this instance.</summary>
		/// <remarks>Does not implement <see cref="IDisposable"/>, is called when the lifetime is terminated.</remarks>
		private void Dispose()
		{
			// Removes the references.
			IAssemblyCookie[] assemblyCookies = _assemblyReferenceManager.References.Values.ToArray();

			if (assemblyCookies.Length <= 0) return;

			_shellLocks.ExecuteWithWriteLock(() =>
			{
				foreach (IAssemblyCookie assemblyCookie in assemblyCookies)
				{
					assemblyCookie.Dispose();
				}
			});
			_assemblyReferenceManager.References.Clear();
		}

		private void AddBaseReferences()
		{
			_assemblyReferenceManager.TryAddReference("mscorlib");
			_assemblyReferenceManager.TryAddReference("System");
			foreach (string assemblyName in _t4Environment.TextTemplatingAssemblyNames)
				_assemblyReferenceManager.TryAddReference(assemblyName);
		}

		public T4FilePsiModule(
			Lifetime lifetime,
			[NotNull] IProjectFile file,
			[NotNull] ChangeManager changeManager,
			[NotNull] IShellLocks shellLocks,
			[NotNull] IT4Environment t4Environment,
			[NotNull] PsiProjectFileTypeCoordinator coordinator
		) : base(
			file.GetProject().NotNull(),
			file.Location.TryMakeRelativeTo(file.GetProject().NotNull().Location).FullPath,
			coordinator,
			file.SelectTargetFrameworkId(t4Environment)
		)
		{
			File = file;
			_lifetime = lifetime;
			lifetime.OnTermination(Dispose);

			var solution = file.GetSolution();
			_psiModules = solution.GetComponent<IPsiModules>();
			AssemblyReferenceInvalidator = solution.GetComponent<T4AssemblyReferenceInvalidator>();
			_changeManager = changeManager;
			_shellLocks = shellLocks;
			_t4Environment = t4Environment;

			var project = file.GetProject();
			var resolveContext = project?.IsMiscFilesProject() != false
				? UniversalModuleReferenceContext.Instance
				: this.GetResolveContextEx(file);
			_assemblyReferenceManager = new T4AssemblyReferenceManager(
				solution.GetComponent<IAssemblyFactory>(),
				file,
				resolveContext
			);
			_projectReferenceManager = new T4ProjectReferenceManager(file, solution);

			changeManager.RegisterChangeProvider(lifetime, ChangeProvider);
			changeManager.AddDependency(lifetime, _psiModules, ChangeProvider);

			var documentManager = solution.GetComponent<DocumentManager>();
			SourceFile = CreateSourceFile(file, documentManager);

			solution.GetComponent<T4DeclaredAssembliesCache>().FileDataChanged.Advise(lifetime, OnFileDataChanged);
			AddBaseReferences();
		}
	}
}
