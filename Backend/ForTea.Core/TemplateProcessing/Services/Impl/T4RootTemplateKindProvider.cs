using GammaJul.ForTea.Core.Psi.Cache;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public sealed class T4RootTemplateKindProvider : IT4RootTemplateKindProvider
	{
		[NotNull]
		private IT4TemplateKindProvider TemplateKindProvider { get; }

		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		public T4RootTemplateKindProvider(
			[NotNull] IT4TemplateKindProvider templateKindProvider,
			[NotNull] IT4FileDependencyGraph graph
		)
		{
			TemplateKindProvider = templateKindProvider;
			Graph = graph;
		}

		public T4TemplateKind GetRootTemplateKind(IProjectFile file) =>
			TemplateKindProvider.GetTemplateKind(Graph.FindBestRoot(file));
	}
}
