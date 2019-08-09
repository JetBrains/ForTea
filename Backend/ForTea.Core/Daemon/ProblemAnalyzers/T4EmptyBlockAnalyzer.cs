using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4CodeBlock),
		HighlightingTypes = new[] {typeof(EmptyBlockHighlighting), typeof(T4EmptyExpressionBlockHighlighting)})]
	public class T4EmptyBlockAnalyzer : ElementProblemAnalyzer<IT4CodeBlock>
	{
		protected override void Run(
			IT4CodeBlock element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			if (!element.GetCodeText().IsNullOrWhitespace()) return;
			consumer.AddHighlighting(CreateHighlighting(element));
		}

		private static IHighlighting CreateHighlighting([NotNull] IT4CodeBlock element)
		{
			if (element is T4ExpressionBlock block) return new T4EmptyExpressionBlockHighlighting(block);
			return new EmptyBlockHighlighting(element);
		}
	}
}
