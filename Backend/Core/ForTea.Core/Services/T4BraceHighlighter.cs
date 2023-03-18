using System;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Daemon.CaretDependentFeatures;
using JetBrains.ReSharper.Feature.Services.Contexts;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Services
{
  [ContainsContextConsumer]
  public class T4BraceHighlighter : ContainingBracesContextHighlighterBase<T4Language>
  {
    [CanBeNull, AsyncContextConsumer]
    public static Action ProcessDataContext(
      Lifetime lifetime,
      [NotNull, ContextKey(typeof(ContextHighlighterPsiFileView.ContextKey))]
      IPsiDocumentRangeView psiDocumentRangeView,
      [NotNull] InvisibleBraceHintManager invisibleBraceHintManager,
      [NotNull] MatchingBraceSuggester matchingBraceSuggester,
      [NotNull] MatchingBraceConsumerFactory consumerFactory,
      [NotNull] HighlightingProlongedLifetime prolongedLifetime
    ) => new T4BraceHighlighter().ProcessDataContextImpl(
      lifetime,
      prolongedLifetime,
      psiDocumentRangeView,
      invisibleBraceHintManager,
      matchingBraceSuggester,
      consumerFactory
    );

    protected override void CollectHighlightings(IPsiView psiView, MatchingHighlightingsConsumer consumer)
    {
      TryConsumeHighlighting<IT4Block>(
        psiView,
        consumer,
        block => block.GetFirstTokenIn(),
        block => block.GetLastTokenIn());
      TryConsumeHighlighting<IT4Macro>(
        psiView,
        consumer,
        macro => macro.LeftParenthesis,
        macro => macro.RightParenthesis);
      TryConsumeHighlighting<IT4EnvironmentVariable>(
        psiView,
        consumer,
        variable => variable.StartPercent,
        variable => variable.EndPercent);
      TryConsumeHighlighting<IT4DirectiveAttribute>(
        psiView,
        consumer,
        attribute => attribute.OpeningQuote,
        attribute => attribute.ClosingQuote);
    }
  }
}