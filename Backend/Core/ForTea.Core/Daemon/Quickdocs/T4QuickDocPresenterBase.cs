using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickDoc;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.UI.RichText;

namespace GammaJul.ForTea.Core.Daemon.Quickdocs
{
  public abstract class T4QuickDocPresenterBase<TExpandable> : IQuickDocPresenter where TExpandable : ITreeNode
  {
    [NotNull] private TExpandable Expandable { get; }
    [NotNull] protected abstract string ExpandableName { get; }

    protected T4QuickDocPresenterBase([NotNull] TExpandable expandable)
    {
      Expandable = expandable;
    }

    public QuickDocTitleAndText GetHtml(PsiLanguageType presentationLanguage)
    {
      var expanded = Expand(Expandable) ?? "(unresolved)";
      return new QuickDocTitleAndText(new RichText($"({ExpandableName}) {expanded}"), "T4 Quickdocs");
    }

    [CanBeNull]
    protected abstract string Expand([NotNull] TExpandable expandable);

    public string GetId()
    {
      return "T4 Quickdoc Placeholder Id";
    }

    public IQuickDocPresenter Resolve(string id)
    {
      return null;
    }

    public void OpenInEditor(string navigationId = "")
    {
    }

    public void ReadMore(string navigationId = "")
    {
    }
  }
}