using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadSafe, typeof(IT4CodeBlock),
    HighlightingTypes = new[] { typeof(EmptyExpressionBlockError) })]
  public class T4EmptyBlockAnalyzer : ElementProblemAnalyzer<IT4CodeBlock>
  {
    protected override void Run(
      IT4CodeBlock element,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer consumer
    )
    {
      if (!element.Code.GetText().IsNullOrWhitespace()) return;
      if (!(element is IT4ExpressionBlock block)) return;
      consumer.AddHighlighting(new EmptyExpressionBlockError(block));
    }
  }
}