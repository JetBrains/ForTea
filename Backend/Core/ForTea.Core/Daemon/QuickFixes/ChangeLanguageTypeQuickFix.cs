using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
  [QuickFix]
  public class ChangeLanguageTypeQuickFix : QuickFixBase
  {
    private NoSupportForVBWarning Highlighting { get; }
    public ChangeLanguageTypeQuickFix(NoSupportForVBWarning highlighting) => Highlighting = highlighting;

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
      var token = Highlighting.Value;
      var newToken = T4ElementFactory.CreateAttributeValue(LanguageAttributeInfo.CSharpLanguageAttributeValue);
      var file = token.GetContainingFile();
      Assertion.AssertNotNull(file, "file != null");

      using (WriteLockCookie.Create(file.IsPhysical()))
      {
        ModificationUtil.ReplaceChild(token, newToken);
      }

      return null;
    }

    public override string Text => "Set language to C#";
    public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
  }
}