using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.Transactions;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCleanup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using JetBrains.Util;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features
{
  [TestFixture]
  [Category("Formatting")]
  [Category("T4")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public class T4FormatterTest : BaseTestWithTextControl
  {
    protected override string RelativeTestDataPath => @"Features\CodeFormatter";

    [TestCase("SmallLoop")]
    [TestCase("StatementAfterTargetLanguage")]
    [TestCase("StatementsAroundTargetLanguage")]
    [TestCase("StatementAndFeatureBlocks")]
    [TestCase("FeatureBlock")]
    [TestCase("RemovingIndent")]
    [TestCase("MisplacedBlockEnd")]
    [TestCase("ExpressionBlock")]
    [TestCase("ComplexFeatureBlock")]
    [TestCase("BrokenFeatureBlock")]
    [TestCase("InnerClasses")]
    [TestCase("SemiBrokenBlock")]
    [TestCase("OneLineStatement")]
    [TestCase("StatementBlockAfterExpressionBlock")]
    [TestCase("ImportDirective")]
    [TestCase("ValueTuple")]
    [TestCase("LargeFile1")]
    public void TestFormatter([NotNull] string name) => DoOneTest(name);

    protected override void DoTest(Lifetime lifetime, IProject testProject)
    {
      var textControl = OpenTextControl(lifetime);
      var document = textControl.Document;
      string newDocumentText = GetTextAfterFormatting(document, textControl);
      document.ReplaceText(document.GetDocumentRange(), newDocumentText);
      CheckTextControl(textControl);
    }

    private string GetTextAfterFormatting([NotNull] IDocument document, [NotNull] ITextControl textControl)
    {
      using (Solution.GetComponent<DocumentTransactionManager>()
               .CreateTransactionCookie(DefaultAction.Rollback, "Temporary change"))
      {
        var file = document.GetPsiSourceFile(Solution);
        var codeCleanup = CodeCleanupService.GetInstance(Solution);
        var codeCleanupSettings = Shell.Instance.GetComponent<CodeCleanupSettingsComponent>();
        var profile = codeCleanupSettings.GetDefaultProfile(CodeCleanupService.DefaultProfileType.REFORMAT);
        var selectionRange = textControl.Selection.OneDocRangeWithCaret();
        codeCleanup.TestCleanupSingleFileWithoutSelectionTracking(file, selectionRange.Length > 0
          ? new DocumentRange(textControl.Document, selectionRange)
          : DocumentRange.InvalidRange, profile, NullProgressIndicator.Create().CreateCodeCleanupProgressIndicator(Solution));
        return document.GetText();
      }
    }
  }
}