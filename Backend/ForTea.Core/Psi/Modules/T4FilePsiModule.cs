using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.Modules.References;
using GammaJul.ForTea.Core.Psi.Modules.References.Impl;
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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Web.Impl.PsiModules;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	/// <summary>PSI module managing a single T4 file.</summary>
	public sealed class T4FilePsiModule : ConcurrentUserDataHolder, IT4FilePsiModule, IDisposable
	{
		[NotNull] public const string Prefix = "[T4]";
		private Lifetime Lifetime { get; }

		[NotNull]
		private IT4AssemblyReferenceManager AssemblyReferenceManager { get; }

		[NotNull]
		private T4ProjectReferenceManager ProjectReferenceManager { get; }

		[NotNull]
		private IPsiModules PsiModules { get; }

		[NotNull]
		private ChangeManager ChangeManager { get; }

		[NotNull]
		private IShellLocks ShellLocks { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		[NotNull]
		private IProjectFile ProjectFile { get; }

		[NotNull]
		private string PersistentId { get; }

		private IChangeProvider ChangeProvider { get; }
		private ISolution Solution { get; }
		public IPsiSourceFile SourceFile { get; }
		public string Name => Prefix + SourceFile.Name;
		public string DisplayName => Prefix + SourceFile.DisplayName;
		public TargetFrameworkId TargetFrameworkId { get; }
		public PsiLanguageType PsiLanguage => T4Language.Instance;
		public ProjectFileType ProjectFileType => T4ProjectFileType.Instance;
		public IModule ContainingProjectModule => Project;
		public IProject Project { get; }

		public IEnumerable<IPsiSourceFile> SourceFiles
		{
			get { yield return SourceFile; }
		}

		public IEnumerable<FileSystemPath> RawReferences => AssemblyReferenceManager.RawReferences;

		public T4FilePsiModule(
			Lifetime lifetime,
			[NotNull] IProjectFile projectFile,
			[NotNull] ChangeManager changeManager,
			[NotNull] IShellLocks shellLocks,
			[NotNull] IT4Environment t4Environment,
			[CanBeNull] TargetFrameworkId primaryTargetFrameworkId
		)
		{
			Lifetime = lifetime;
			lifetime.AddDispose(this);
			ProjectFile = projectFile;
			Solution = ProjectFile.GetSolution();
			PsiModules = Solution.GetComponent<IPsiModules>();
			PsiServices = Solution.GetComponent<IPsiServices>();
			ChangeManager = changeManager;
			ShellLocks = shellLocks;
			ChangeProvider = new FakeChangeProvider();
			TargetFrameworkId = t4Environment.SelectTargetFrameworkId(primaryTargetFrameworkId, projectFile);
			Project = ProjectFile.GetProject().NotNull();
			var resolveContext = Project.IsMiscFilesProject()
				? UniversalModuleReferenceContext.Instance
				: this.GetResolveContextEx(ProjectFile);
			Assertion.Assert(resolveContext.TargetFramework == TargetFrameworkId, "Failed to select TargetFrameworkId");
			var documentManager = Solution.GetComponent<DocumentManager>();
			SourceFile = CreateSourceFile(ProjectFile, documentManager, resolveContext);
			AssemblyReferenceManager = new T4AssemblyReferenceManager(
				Solution.GetComponent<IAssemblyFactory>(),
				SourceFile,
				ProjectFile,
				resolveContext,
				shellLocks
			);
			ProjectReferenceManager = new T4ProjectReferenceManager(ProjectFile, Solution);

			changeManager.RegisterChangeProvider(lifetime, ChangeProvider);
			changeManager.AddDependency(lifetime, PsiModules, ChangeProvider);
			Solution.GetComponent<T4DeclaredAssembliesManager>().FileDataChanged.Advise(lifetime, OnFileDataChanged);
			PersistentId = BuildPersistentId(primaryTargetFrameworkId);
			ChangeManager.ExecuteAfterChange(() =>
			{
				AssemblyReferenceManager.AddBaseReferences();
				NotifyModuleChange();
			});
		}

		[NotNull]
		private string BuildPersistentId([CanBeNull] TargetFrameworkId primaryTargetFrameworkId) =>
			$"{Prefix}(path: {ProjectFile.GetPersistentID()}, containing project target framework id: {primaryTargetFrameworkId?.UniqueString ?? "null"})";

		private void OnFileDataChanged(Pair<IPsiSourceFile, T4DeclaredAssembliesDiff> pair)
		{
			(IPsiSourceFile first, T4DeclaredAssembliesDiff second) = pair;
			if (first != SourceFile) return;
			if (ShellLocks.IsWriteAccessAllowed())
				OnFileDataChanged(second);
			else
			{
				ShellLocks.ExecuteOrQueueEx(
					Lifetime,
					"T4PsiModuleOnFileDataChanged",
					() => ShellLocks.ExecuteWithWriteLock(() => OnFileDataChanged(second))
				);
			}
		}

		/// <summary>Called when the associated data file changed: added/removed assemblies or includes.</summary>
		/// <param name="dataDiff">The difference between the old and new data.</param>
		private void OnFileDataChanged([NotNull] T4DeclaredAssembliesDiff dataDiff)
		{
			ShellLocks.AssertWriteAccessAllowed();
			if (!AssemblyReferenceManager.ProcessDiff(dataDiff)) return;
			NotifyModuleChange();
		}

		private void NotifyModuleChange()
		{
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.Modified);
			// TODO: get rid of this queuing?
			ShellLocks.ExecuteOrQueueEx(
				"T4PsiModuleChange",
				() => ChangeManager.ExecuteAfterChange(
					() => ShellLocks.ExecuteWithWriteLock(
						() => ChangeManager.OnProviderChanged(
							ChangeProvider,
							changeBuilder.Result,
							SimpleTaskExecutor.Instance
						)
					)
				)
			);
		}

		public IEnumerable<IPsiModuleReference> GetReferences(IModuleReferenceResolveContext _)
		{
			var references = new PsiModuleReferenceAccumulator(TargetFrameworkId);
			var moduleReferences = AssemblyReferenceManager
				.AssemblyReferences
				.Concat(AssemblyReferenceManager.ProjectReferences)
				.SelectNotNull(assembly => PsiModules.GetPrimaryPsiModule(assembly, TargetFrameworkId))
				.Select(it => new PsiModuleReference(it));
			references.AddRange(moduleReferences);
			references.AddRange(ProjectReferenceManager.GetProjectReference());
			return references.GetReferences();
		}

		[NotNull]
		private PsiProjectFile CreateSourceFile(
			[NotNull] IProjectFile projectFile,
			[NotNull] DocumentManager documentManager,
			[NotNull] IModuleReferenceResolveContext resolveContext
		) => new PsiProjectFile(
			this,
			projectFile,
			(pf, sf) => new T4PsiProjectFileProperties(pf, sf, true),
			JetFunc<IProjectFile, IPsiSourceFile>.True,
			documentManager,
			resolveContext
		);

		public void Dispose() => AssemblyReferenceManager.Dispose();
		public IPsiServices GetPsiServices() => PsiServices;
		public ISolution GetSolution() => Solution;
		public ICollection<PreProcessingDirective> GetAllDefines() => EmptyList<PreProcessingDirective>.Instance;
		public bool IsValid() => Project.IsValid() && PsiServices.Modules.HasModule(this);
		public string GetPersistentID() => PersistentId;
	}
}
