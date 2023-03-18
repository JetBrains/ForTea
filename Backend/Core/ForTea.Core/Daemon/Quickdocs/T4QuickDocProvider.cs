using System;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.DataContext;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.QuickDoc;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Quickdocs
{
  [QuickDocProvider(-100)]
  public class T4QuickDocProvider : IQuickDocProvider
  {
    public bool CanNavigate(IDataContext context)
    {
      var token = TryFindToken(context);
      return token != null
             && (token.GetParentOfType<IT4Macro>() != null
                 || token.GetParentOfType<IT4EnvironmentVariable>() != null);
    }

    private static ITreeNode TryFindToken(IDataContext context)
    {
      var editorContext = context.GetData(DocumentModelDataConstants.EDITOR_CONTEXT);
      if (editorContext == null) return null;
      var sourceFile = context.GetData(PsiDataConstants.SOURCE_FILE);
      var file = sourceFile?.GetPsiFile(T4Language.Instance, editorContext.CaretOffset);
      return file?.FindTokenAt(editorContext.CaretOffset);
    }

    public void Resolve(IDataContext context, Action<IQuickDocPresenter, PsiLanguageType> resolved)
    {
      var token = TryFindToken(context).NotNull();
      var macro = token.GetParentOfType<IT4Macro>();
      if (macro != null)
      {
        var solution = context.GetData(ProjectModelDataConstants.SOLUTION).NotNull();
        var resolver = solution.GetComponent<IT4MacroResolver>();
        resolved(new T4MacroQuickDocPresenter(macro, resolver), T4Language.Instance);
        return;
      }

      var environmentVariable = token.GetParentOfType<IT4EnvironmentVariable>().NotNull();
      resolved(new T4EnvironmentQuickDocPresenter(environmentVariable), T4Language.Instance);
    }
  }
}