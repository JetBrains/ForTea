using GammaJul.ForTea.Core.Tree;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	public sealed class T4RangeTranslatorTest : BaseTestWithSingleProject
	{
		[Test]
		public void TestRangesInIntermediateInclude() => WithSingleProject(
			new[] {"Includer.tt", "Include.ttinclude", "Include2.ttinclude"},
			(lifetime, solution, project) =>
			{
				var includeProjectFile = project.GetSubFiles("Include.ttinclude").Single();
				var psiFile = (IT4File) includeProjectFile.GetPrimaryPsiFile().NotNull();
				var translator = ((IT4FileLikeNode) psiFile).DocumentRangeTranslator;
				var includerDocument = includeProjectFile.GetDocument();
				var originalDocumentRange = new DocumentRange(new DocumentOffset(includerDocument, 119));
				var treeRange = translator.Translate(originalDocumentRange);
				var resultingRange = translator.Translate(treeRange);
				Assert.AreEqual(originalDocumentRange, resultingRange);
			}
		);
	}
}
