using GammaJul.ForTea.Core.Psi.Invalidation;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public sealed class T4TemplateKindProvider : IT4TemplateKindProvider
	{
		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		public T4TemplateKindProvider([NotNull] IT4FileDependencyGraph graph) => Graph = graph;

		public T4TemplateKind GetTemplateKind(IProjectFile file)
		{
			var properties = file.Properties as ProjectFileProperties;
			string customTool = properties?.CustomTool;
			return T4TemplateManagerConstants.ToTemplateKind(customTool);
		}

		public T4TemplateKind GetRootTemplateKind(IProjectFile file) =>
			GetTemplateKind(Graph.FindBestRoot(file));
	}
}
