using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadUnsafe, typeof(IT4AttributeValue), HighlightingTypes =
    new[] { typeof(InvalidAttributeValueError) })]
  public sealed class T4InvalidAttributeValueAnalyzer : ElementProblemAnalyzer<IT4AttributeValue>
  {
    protected override void Run(
      IT4AttributeValue element,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer consumer
    )
    {
      if (!(element.Parent is IT4DirectiveAttribute attribute)) return;
      if (!(attribute.Parent is IT4Directive directive)) return;
      var attributeInfo = T4DirectiveInfoManager
        .GetDirectiveByName(directive.Name.GetText())
        ?.GetAttributeByName(attribute.Name.GetText());
      if (attributeInfo?.IsValid(element.GetText()) != false) return;
      consumer.AddHighlighting(new InvalidAttributeValueError(element));
    }
  }
}