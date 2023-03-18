using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(typeof(IT4Directive), HighlightingTypes =
    new[] { typeof(MissingRequiredAttributeError) })]
  public sealed class T4MissingRequiredAttributeAnalyzer : ElementProblemAnalyzer<IT4Directive>
  {
    protected override void Run(
      IT4Directive element,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer consumer
    )
    {
      var nameToken = element.Name;
      if (nameToken == null) return;
      var directiveInfo = T4DirectiveInfoManager.GetDirectiveByName(nameToken.GetText());
      var missingInfos = directiveInfo?.SupportedAttributes
        .Where(requiredAttributeInfo =>
          requiredAttributeInfo.IsRequired
          && !element
            .AttributesEnumerable
            .Select(attribute => attribute.Name)
            .Any(name => name
              .GetText()
              .Equals(requiredAttributeInfo.Name, StringComparison.OrdinalIgnoreCase)
            )
        )
        .AsList();
      if (missingInfos?.Any() != true) return;
      consumer.AddHighlighting(new MissingRequiredAttributeError(nameToken, CreateMessage(missingInfos)));
    }

    [NotNull]
    private static string CreateMessage([NotNull, ItemNotNull] IList<DirectiveAttributeInfo> infos) =>
      infos.Count switch
      {
        _ when infos.Count <= 0 => throw new InvalidOperationException(),
        1 => "Missing required attribute: {0}".FormatEx(infos.Single().Name),
        _ => infos
          .Take(infos.Count - 1)
          .Aggregate(
            new StringBuilder("Missing required attributes: "),
            (builder, info) => builder.Append(info.Name)
          )
          .Append(" and ")
          .Append(infos.Last())
          .ToString()
      };
  }
}