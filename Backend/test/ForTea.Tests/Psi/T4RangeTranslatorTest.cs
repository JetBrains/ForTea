using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
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

		private void DoTest([NotNull] params string[] fileNames) =>
			WithSingleProject(fileNames, (lifetime, solution, project) =>
			{
				using var cookie = ReadLockCookie.Create();
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
	}
}
