using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4CodeBlock), HighlightingTypes = new[] {typeof(EmptyBlockHighlighting)})]
	public class T4EmptyBlockAnalyzer : ElementProblemAnalyzer<IT4CodeBlock>
	{
		protected override void Run(
			IT4CodeBlock element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			if (element is T4FeatureBlock) return;
			if (!(element is T4CodeBlock block)) return;
			if (!block.GetCodeText().IsNullOrWhitespace()) return;
			consumer.AddHighlighting(new EmptyBlockHighlighting(block));
		}
	}
}
