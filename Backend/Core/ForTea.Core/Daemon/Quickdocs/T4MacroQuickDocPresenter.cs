using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.Quickdocs
{
  public class T4MacroQuickDocPresenter : T4QuickDocPresenterBase<IT4Macro>
  {
    [NotNull] private IT4MacroResolver Resolver { get; }

    public T4MacroQuickDocPresenter([NotNull] IT4Macro expandable, [NotNull] IT4MacroResolver resolver) :
      base(expandable)
    {
      Resolver = resolver;
    }

    protected override string ExpandableName => "macro";

    protected override string Expand(IT4Macro macro)
    {
      var projectFile = macro
        .GetParentOfType<IT4FileLikeNode>()
        .NotNull()
        .PhysicalPsiSourceFile
        .ToProjectFile()
        .NotNull();
      string name = macro.RawAttributeValue?.GetText();
      if (name == null) return null;
      var macros = Resolver.ResolveHeavyMacros(new[] { name }, projectFile);
      return macros.ContainsKey(name) ? macros[name] : null;
    }
  }
}