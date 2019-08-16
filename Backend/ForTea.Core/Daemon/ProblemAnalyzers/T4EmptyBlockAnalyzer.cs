using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4CodeBlock),
		HighlightingTypes = new[] {typeof(T4EmptyExpressionBlockHighlighting)})]
	public class T4EmptyBlockAnalyzer : ElementProblemAnalyzer<IT4CodeBlock>
	{
		protected override void Run(
			IT4CodeBlock element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			if (!element.GetCodeText().IsNullOrWhitespace()) return;
			if (!(element is T4ExpressionBlock block)) return;
			consumer.AddHighlighting(new T4EmptyExpressionBlockHighlighting(block));
		}
	}
}
