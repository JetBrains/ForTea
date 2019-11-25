using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules.References
{
	/// <summary>
	/// T4 files can be included into preprocessed files.
	/// In that case the C# code in them will appear in the project where the includer resides.
	/// Therefore, the symbols in the included file should be resolved in the context of that project.
	/// To do that, we need to reference that project in T4 PSI module and allow its internals to be visible.
	/// This class handles that.
	///
	/// That project might want to be able see the symbols from the included T4 file, too.
	/// For that, however, no additional effort is necessary
	/// since the includer file will contain all the members of the included.
	/// </summary>
	public sealed class T4ProjectReferenceManager
	{
		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		[NotNull]
		private IT4TemplateKindProvider TemplateKindProvider { get; }

		[NotNull]
		private IProjectFile ProjectFile { get; }

		[NotNull]
		private ISolution Solution { get; }

		[CanBeNull]
		private IProject IncluderContainerProject
		{
			get
			{
				var root = Graph.FindBestRoot(ProjectFile);
				if (!TemplateKindProvider.IsPreprocessedTemplate(root)) return null;
				return root.GetProject();
			}
		}

		public T4ProjectReferenceManager(
			[NotNull] IProjectFile projectFile,
			[NotNull] ISolution solution
		)
		{
			Graph = solution.GetComponent<IT4FileDependencyGraph>();
			TemplateKindProvider = solution.GetComponent<IT4TemplateKindProvider>();
			ProjectFile = projectFile;
			Solution = solution;
		}

		[NotNull, ItemNotNull]
		public IEnumerable<IPsiModuleReference> GetProjectReference()
		{
			var project = IncluderContainerProject;
			if (project == null) return EmptyList<IPsiModuleReference>.Enumerable;
			var targetFrameworkId = project.GetCurrentTargetFrameworkId();
			return Solution
				.PsiModules()
				.GetPsiModulesToReference(project, targetFrameworkId)
				.Where(it => it.IsValid())
				.Select(it => new PsiModuleReference(it));
		}
	}
}
