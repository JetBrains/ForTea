using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(typeof(IT4UnknownDirective), HighlightingTypes =
    new[] { typeof(UnexpectedDirectiveWarning) })]
  public class T4UnexpectedDirectiveAnalyzer : ElementProblemAnalyzer<IT4UnknownDirective>
  {
    protected override void Run(
      IT4UnknownDirective element,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer consumer
    )
    {
      var name = element.Name;
      if (name == null) return;
      consumer.AddHighlighting(new UnexpectedDirectiveWarning(name));
    }
  }
}