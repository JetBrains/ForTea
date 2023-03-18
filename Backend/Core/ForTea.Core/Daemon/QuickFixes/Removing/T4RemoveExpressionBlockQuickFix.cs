using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
  [QuickFix]
  public class T4RemoveExpressionBlockQuickFix :
    T4RemoveBlockQuickFixBase<IT4ExpressionBlock, EmptyExpressionBlockError>
  {
    public override string Text => "Remove empty expression block";
    protected override IT4ExpressionBlock Node => Highlighting.Block;

    public T4RemoveExpressionBlockQuickFix([NotNull] EmptyExpressionBlockError highlighting) : base(highlighting)
    {
    }

    protected override bool ShouldRemove(ITokenNode nextToken) => false;
  }
}