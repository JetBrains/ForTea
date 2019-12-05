using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	/// <summary>Provides <see cref="IT4FilePsiModule"/> for T4 files opened inside the solution.</summary>
	internal sealed class T4ProjectPsiModuleHandler : DelegatingProjectPsiModuleHandler
	{
		[NotNull] private readonly T4PsiModuleProvider _t4PsiModuleProvider;

		[NotNull]
		private IT4TemplateKindProvider TemplateKindProvider { get; }

		public override IList<IPsiModule> GetAllModules()
		{
			var modules = new List<IPsiModule>(base.GetAllModules());
			modules.AddRange(_t4PsiModuleProvider.GetModules());
			return modules;
		}

		public override void OnProjectFileChanged(
			IProjectFile projectFile,
			FileSystemPath oldLocation,
			PsiModuleChange.ChangeType changeType,
			PsiModuleChangeBuilder changeBuilder
		)
		{
			var requestedChange = _t4PsiModuleProvider.OnProjectFileChanged(projectFile, changeType, changeBuilder);
			if (requestedChange == null) return;
			base.OnProjectFileChanged(projectFile, oldLocation, requestedChange.Value, changeBuilder);
		}

		public override IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile)
			=> base.GetPsiSourceFilesFor(projectFile).Concat(_t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile));

		public T4ProjectPsiModuleHandler(
			Lifetime lifetime,
			[NotNull] IProjectPsiModuleHandler handler,
			[NotNull] ChangeManager changeManager,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IProject project,
			[NotNull] IT4TemplateKindProvider templateKindProvider
		) : base(handler)
		{
			TemplateKindProvider = templateKindProvider;
			_t4PsiModuleProvider = new T4PsiModuleProvider(
				lifetime,
				project.Locks,
				changeManager,
				t4Environment,
				templateKindProvider
			);
		}

		public override bool InternalsVisibleTo(IPsiModule moduleTo, IPsiModule moduleFrom)
		{
			if (!(moduleTo is T4FilePsiModule t4Module)) return base.InternalsVisibleTo(moduleTo, moduleFrom);
			if (moduleFrom is IAssemblyPsiModule) return false;
			if (!(moduleFrom is IProjectPsiModule projectModule)) return false;
			var projectFile = t4Module.SourceFile.ToProjectFile().NotNull();
			var root = projectFile.GetSolution().GetComponent<IT4FileDependencyGraph>().FindBestRoot(projectFile);
			if (!TemplateKindProvider.IsPreprocessedTemplate(root)) return false;
			return Equals(root.GetProject(), projectModule.Project);
		}
	}
}
