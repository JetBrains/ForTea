using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
  [QuickFix]
  public sealed class T4RemoveRedundantAssemblyDirectiveQuickFix :
    T4RemoveBlockQuickFixBase<IT4AssemblyDirective, RedundantAssemblyWarning>
  {
    public override string Text => "Remove redundant assembly reference";

    public T4RemoveRedundantAssemblyDirectiveQuickFix(
      [NotNull] RedundantAssemblyWarning highlighting
    ) : base(highlighting)
    {
    }

    protected override IT4AssemblyDirective Node => Highlighting.Assembly;
  }
}