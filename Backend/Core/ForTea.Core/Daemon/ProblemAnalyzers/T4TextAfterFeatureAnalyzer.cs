using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadUnsafe, typeof(IT4Token), HighlightingTypes = new[] { typeof(TextAfterFeatureError) })]
  public sealed class T4TextAfterFeatureAnalyzer : ElementProblemAnalyzer<IT4Token>
  {
    protected override void Run(
      IT4Token element,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer consumer
    )
    {
      if (element.GetTokenType() != T4TokenNodeTypes.RAW_TEXT) return;
      if (!HasFeatureBlocks(element)) return;
      if (HasFeatureBlocksAhead(element)) return;
      if (IsInFeatureBlock(element)) return;
      consumer.AddHighlighting(new TextAfterFeatureError(element));
    }

    private static bool HasFeatureBlocks([NotNull] IT4Token element) =>
      element.GetParentOfType<IT4FileLikeNode>().NotNull().Blocks.OfType<IT4FeatureBlock>().Any();

    private static bool IsInFeatureBlock([NotNull] IT4Token element) =>
      element.GetParentOfType<IT4FeatureBlock>() != null;

    private static bool HasFeatureBlocksAhead([NotNull] IT4Token element) =>
      element.NextTokens().Any(token => token.GetParentOfType<IT4FeatureBlock>() != null);
  }
}