using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AssemblyDirective), HighlightingTypes =
		new[] {typeof(IgnoredAssemblyDirectiveWarning)})]
	public sealed class T4IgnoredAssemblyDirectiveAnalyzer : ElementProblemAnalyzer<IT4AssemblyDirective>
	{
		protected override void Run(
			IT4AssemblyDirective element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			var sourceFile = element.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var solution = sourceFile.GetSolution();
			var templateKindProvider = solution.GetComponent<IT4TemplateKindProvider>();
			if (!templateKindProvider.IsPreprocessedTemplate(projectFile)) return;
			consumer.AddHighlighting(new IgnoredAssemblyDirectiveWarning(element));
		}
	}
}
