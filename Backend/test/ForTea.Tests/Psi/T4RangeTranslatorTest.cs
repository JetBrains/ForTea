using GammaJul.ForTea.Core.Tree;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	public sealed class T4RangeTranslatorTest : BaseTestWithSingleProject
	{
		[Test]
		public void TestThatTwoTranslationsAreInverseFunctions()
		{
			var fileNames = new[] {"Includer.tt", "Include.ttinclude", "Include2.ttinclude"};
			WithSingleProject(fileNames, (lifetime, solution, project) =>
			{
				using var cookie = ReadLockCookie.Create();
				foreach (string fileName in fileNames)
				{
					var projectFile = project.GetSubFiles(fileName).Single();
					var psiFile = (IT4File) projectFile.GetPrimaryPsiFile().NotNull();
					var translator = ((IT4FileLikeNode) psiFile).DocumentRangeTranslator;
					var document = projectFile.GetDocument();
					// Do not want to analyze edge cases here
					for (int offset = 1; offset < document.GetTextLength() - 2; offset += 1)
					{
						var originalDocumentRange = new DocumentRange(new DocumentOffset(document, offset));
						var treeRange = translator.Translate(originalDocumentRange);
						var resultingRange = translator.Translate(treeRange);
						Assert.AreEqual(originalDocumentRange, resultingRange);
					}
				}
			});
		}
	}
}
