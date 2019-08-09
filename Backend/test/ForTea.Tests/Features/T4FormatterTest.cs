using GammaJul.ForTea.Core.Psi;
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
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public class T4FormatterTest : BaseTestWithTextControl
	{
		protected override string RelativeTestDataPath => @"Features\CodeFormatter";

		[TestCase("SmallLoop")]
		[TestCase("StatementAfterTargetLanguage")]
		[TestCase("StatementsAroundTargetLanguage")]
		[TestCase("StatementAndFeatureBlocks", Ignore = "Not implemented")]
		[TestCase("FeatureBlock")]
		[TestCase("RemovingIndent")]
		[TestCase("MisplacedBlockEnd")]
		[TestCase("ExpressionBlock", Ignore = "Not implemented")]
		[TestCase("ComplexFeatureBlock")]
		[TestCase("BrokenFeatureBlock")]
		public void TestFormatter([NotNull] string name) => DoOneTest(name);

		protected override void DoTest(Lifetime lifetime, IProject testProject)
		{
			var textControl = OpenTextControl(lifetime);
			var document = textControl.Document;
			int caretOffset = textControl.Caret.Offset();
			string newDocumentText = GetTextAfterFormatting(document, textControl, ref caretOffset);
			document.ReplaceText(TextRange.FromLength(document.GetTextLength()), newDocumentText);
			if (caretOffset >= 0)
			{
				textControl.Caret.MoveTo(caretOffset, CaretVisualPlacement.Generic);
			}

			CheckTextControl(textControl);
		}

		private string GetTextAfterFormatting(
			[NotNull] IDocument document,
			[NotNull] ITextControl textControl,
			ref int caretOffset
		)
		{
			using (Solution.GetComponent<DocumentTransactionManager>()
				.CreateTransactionCookie(DefaultAction.Rollback, "Temporary change"))
			{
				var file = document.GetPsiSourceFile(Solution);
				var codeCleanup = CodeCleanup.GetInstance(Solution);
				var codeCleanupSettings = Shell.Instance.GetComponent<CodeCleanupSettingsComponent>();
				var profile = codeCleanupSettings.GetDefaultProfile(CodeCleanup.DefaultProfileType.REFORMAT);
				var selectionRange = textControl.Selection.OneDocRangeWithCaret();
				codeCleanup.Run(file, selectionRange.Length > 0
					? new DocumentRange(textControl.Document, selectionRange)
					: DocumentRange.InvalidRange, ref caretOffset, profile, NullProgressIndicator.Create());
				return document.GetText();
			}
		}
	}
}
