using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.Modules.References;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
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
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	/// <summary>PSI module managing a single T4 file.</summary>
	public sealed class T4FilePsiModule : ConcurrentUserDataHolder, IT4FilePsiModule
	{
		[NotNull]
		private const string Prefix = "[T4]";

		private Lifetime Lifetime { get; }

		[NotNull]
		private T4AssemblyReferenceManager AssemblyReferenceManager { get; }

		[NotNull]
		private T4ProjectReferenceManager ProjectReferenceManager { get; }

		[NotNull]
		private IPsiModules PsiModules { get; }

		[NotNull]
		private ChangeManager ChangeManager { get; }

		[NotNull]
		private IShellLocks ShellLocks { get; }

		[NotNull]
		private IT4Environment T4Environment { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		[NotNull]
		private T4AssemblyReferenceInvalidator AssemblyReferenceInvalidator { get; }

		[NotNull]
		private IProjectFile ProjectFile { get; }

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

		public IEnumerable<IAssembly> RawReferences =>
			AssemblyReferenceManager.References.Values.SelectNotNull(cookie => cookie.Assembly);

		public T4FilePsiModule(
			Lifetime lifetime,
			[NotNull] IProjectFile projectFile,
			[NotNull] ChangeManager changeManager,
			[NotNull] IShellLocks shellLocks,
			[NotNull] IT4Environment t4Environment
		)
		{
			Lifetime = lifetime;
			lifetime.OnTermination(Dispose);
			ProjectFile = projectFile;
			Solution = ProjectFile.GetSolution();
			PsiModules = Solution.GetComponent<IPsiModules>();
			PsiServices = Solution.GetComponent<IPsiServices>();
			AssemblyReferenceInvalidator = Solution.GetComponent<T4AssemblyReferenceInvalidator>();
			ChangeManager = changeManager;
			ShellLocks = shellLocks;
			T4Environment = t4Environment;
			ChangeProvider = new FakeChangeProvider();
			TargetFrameworkId = ProjectFile.SelectTargetFrameworkId(t4Environment);
			Project = ProjectFile.GetProject().NotNull();
			var resolveContext = Project.IsMiscFilesProject()
				? UniversalModuleReferenceContext.Instance
				: this.GetResolveContextEx(ProjectFile);
			AssemblyReferenceManager = new T4AssemblyReferenceManager(
				Solution.GetComponent<IAssemblyFactory>(),
				ProjectFile,
				resolveContext
			);
			ProjectReferenceManager = new T4ProjectReferenceManager(ProjectFile, Solution);

			changeManager.RegisterChangeProvider(lifetime, ChangeProvider);
			changeManager.AddDependency(lifetime, PsiModules, ChangeProvider);

			var documentManager = Solution.GetComponent<DocumentManager>();
			SourceFile = CreateSourceFile(ProjectFile, documentManager);
			Solution.GetComponent<T4DeclaredAssembliesManager>().FileDataChanged.Advise(lifetime, OnFileDataChanged);
			AddBaseReferences();
		}

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
			bool hasChanges = AssemblyReferenceInvalidator.InvalidateAssemblies(
				dataDiff,
				ProjectFile,
				AssemblyReferenceManager
			);

			if (!hasChanges) return;

			// tells the world the module has changed
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.Modified);
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
			var moduleReferences = RawReferences
				.SelectNotNull(assembly => PsiModules.GetPrimaryPsiModule(assembly, TargetFrameworkId))
				.Select(it => new PsiModuleReference(it));
			references.AddRange(moduleReferences);
			references.AddRange(ProjectReferenceManager.GetProjectReference());
			return references.GetReferences();
		}

		[NotNull]
		private PsiProjectFile CreateSourceFile(
			[NotNull] IProjectFile projectFile,
			[NotNull] DocumentManager documentManager
		) => new PsiProjectFile(
			this,
			projectFile,
			(pf, sf) => new T4PsiProjectFileProperties(pf, sf, true),
			JetFunc<IProjectFile, IPsiSourceFile>.True,
			documentManager,
			AssemblyReferenceManager.ResolveContext
		);

		/// <summary>Disposes this instance.</summary>
		/// <remarks>Does not implement <see cref="IDisposable"/>, is called when the lifetime is terminated.</remarks>
		private void Dispose()
		{
			// Removes the references.
			var assemblyCookies = AssemblyReferenceManager.References.Values.ToArray();
			if (assemblyCookies.Length <= 0) return;
			ShellLocks.ExecuteWithWriteLock(
				() =>
				{
					foreach (var assemblyCookie in assemblyCookies)
					{
						assemblyCookie.Dispose();
					}
				}
			);
			AssemblyReferenceManager.References.Clear();
		}

		private void AddBaseReferences()
		{
			TryAddReference("mscorlib");
			TryAddReference("System");
			foreach (string assemblyName in T4Environment.TextTemplatingAssemblyNames)
			{
				TryAddReference(assemblyName);
			}
		}

		private void TryAddReference([NotNull] string name) =>
			AssemblyReferenceManager.TryAddReference(new T4PathWithMacros(name, SourceFile, ProjectFile, GetSolution()));

		public IPsiServices GetPsiServices() => PsiServices;
		public ISolution GetSolution() => Solution;
		public ICollection<PreProcessingDirective> GetAllDefines() => EmptyList<PreProcessingDirective>.Instance;
		public bool IsValid() => Project.IsValid() && PsiServices.Modules.HasModule(this);
		public string GetPersistentID() => Prefix + ProjectFile.GetPersistentID();
	}
}
