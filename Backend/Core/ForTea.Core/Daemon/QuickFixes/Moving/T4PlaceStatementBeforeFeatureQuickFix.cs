using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Moving
{
  [QuickFix]
  public sealed class T4PlaceStatementBeforeFeatureQuickFix :
    PlaceBeforeFeatureQuickFixBase<IT4StatementBlock, StatementAfterFeatureError>
  {
    protected override IT4StatementBlock Node => Highlighting
      .BlockStart
      .GetParentOfType<IT4StatementBlock>()
      .NotNull();

    public T4PlaceStatementBeforeFeatureQuickFix(
      [NotNull] StatementAfterFeatureError highlighting
    ) : base(highlighting)
    {
    }
  }
}