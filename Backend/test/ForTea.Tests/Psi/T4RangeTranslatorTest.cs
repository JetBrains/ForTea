using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.UI.ActionSystem.Text;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using JetBrains.Util;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	public sealed class T4RangeTranslatorTest : BaseTestWithSingleProject
	{
		[Test]
		public void TestIncludeLine() =>
			DoTest("Includer.tt", "Include.ttinclude", "Include2.ttinclude");

		[Test]
		public void TestBigTree() => DoTest(
			"Template1.tt",
			"Template2.tt",
			"Include.ttinclude",
			"Include2.ttinclude",
			"Include3.ttinclude",
			"Include4.ttinclude",
			"Include5.ttinclude"
		);

		[Test]
		public void TestBackspaceInCSharp() => WithSingleProject(
			new[] {"Includer3.tt", "Include6.ttinclude"},
			(lifetime, solution, project) =>
			{
				var projectFile = project.GetSubFiles("Include6.ttinclude").Single();
				var sourceFile = projectFile.ToSourceFiles().Single();
				RunGuarded(() => ExecuteWithGold(sourceFile, writer =>
				{
					var editorManager = Solution.GetComponent<IEditorManager>();
					var textControl = editorManager
						.OpenProjectFileAsync(projectFile, OpenFileOptions.DefaultActivate)
						.Result
						.NotNull();
					lifetime.OnTermination(() => RunGuarded(() => editorManager.CloseTextControl(textControl)));
					// The beginning of the newline
					textControl.Caret.MoveTo(83, CaretVisualPlacement.Generic);
					textControl.EmulateAction(TextControlActions.ActionIds.Backspace);
					solution.GetComponent<IPsiFiles>().CommitAllDocuments();
					writer.Write(textControl.Document.GetText());
				}));
			}
		);

		private void DoTest([NotNull] params string[] fileNames) =>
			WithSingleProject(fileNames, (lifetime, solution, project) =>
			{
				foreach (string fileName in fileNames)
				{
					var projectFile = project.GetSubFiles(fileName).Single();
					var psiFile = (IT4File) projectFile.GetPrimaryPsiFile().NotNull();
					var translator = ((IT4FileLikeNode) psiFile).DocumentRangeTranslator;
					var document = projectFile.GetDocument();
					for (int offset = 1; offset < document.GetTextLength() - 1; offset += 1)
					{
						// Do not want to analyze edge cases here
						if (IsEdgeOfIncludeCase(document, offset)) continue;
						var originalDocumentRange = new DocumentRange(new DocumentOffset(document, offset));
						var treeRange = translator.Translate(originalDocumentRange);
						var resultingRange = translator.Translate(treeRange);
						Assert.AreEqual(originalDocumentRange, resultingRange);
					}
				}
			});

		private static bool IsEdgeOfIncludeCase([NotNull] IDocument document, int offset)
		{
			Assert.That(offset < document.GetTextLength());
			const string template = ".ttinclude\" #>";
			int start = offset - template.Length;
			if (start < 0) return false;
			return document.GetText(new TextRange(start, offset)).Equals(template);
		}

		private new void WithSingleProject(
			[NotNull] IEnumerable<string> fileNames,
			[NotNull] Action<Lifetime, ISolution, IProject> F
		) => base.WithSingleProject(fileNames, (lifetime, solution, project) =>
		{
			using var cookie = WriteLockCookie.Create();
			F(lifetime, solution, project);
		});
	}
}
