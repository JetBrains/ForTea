using System.Collections.Generic;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadUnsafe, typeof(IT4OutputDirective), HighlightingTypes = new[] { typeof(IllegalCharacterError) })]
  public class T4IllegalCharacterAnalyzer : T4AttributeValueProblemAnalyzerBase<IT4OutputDirective>
  {
    [NotNull] private static IReadOnlyList<char> AllowedCharacters { get; } = new[] { '.' };

    protected override DirectiveAttributeInfo GetTargetAttribute() =>
      T4DirectiveInfoManager.Output.ExtensionAttribute;

    protected override void DoRun(IT4AttributeValue element, IHighlightingConsumer consumer)
    {
      var elementStart = element.GetDocumentStartOffset();
      string text = element.GetText();
      for (int index = 0; index < text.Length; index++)
      {
        if (IsLegal(text[index])) continue;
        var range = new DocumentRange(elementStart + index, elementStart + index + 1);
        var highlighting = new IllegalCharacterError(element, range);
        consumer.AddHighlighting(highlighting);
      }
    }

    private static bool IsLegal(char c) => c.IsLetterOrDigitFast() || AllowedCharacters.Contains(c);
  }
}