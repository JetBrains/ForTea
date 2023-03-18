using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
  [QuickFix]
  public sealed class T4RemoveDirectiveQuickFix : T4RemoveBlockQuickFixBase<IT4Directive, IgnoredDirectiveWarning>
  {
    public override string Text => "Remove directive";
    protected override IT4Directive Node => Highlighting.Directive;

    public T4RemoveDirectiveQuickFix([NotNull] IgnoredDirectiveWarning highlighting) : base(highlighting)
    {
    }
  }
}