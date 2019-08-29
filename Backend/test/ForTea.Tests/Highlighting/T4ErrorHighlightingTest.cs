using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4ErrorHighlightingTest : T4HighlightingTestBase
	{
		protected override string RelativeTestDataPath => @"Highlighting\Error";

		[TestCase("CSharpUnresolvedReference")]
		[TestCase("AfterLastFeature")]
		[TestCase("IgnoredAssemblyDirectiveHighlighting", Ignore = "We cannot distinguish template types yet")]
		[TestCase("InvalidAttributeValue")]
		[TestCase("MissingRequiredAttribute")]
		[TestCase("MissingToken", Ignore = "Error recovery not implemented")]
		[TestCase("StatementAfterFeature")]
		[TestCase("EmptyExpressionBlock")]
		[TestCase("RecursiveInclude")]
		[TestCase("UnknownEncoding")]
		[TestCase("PossibleLanguages")]
		[TestCase("IllegalCharacter")]
		public void TestHighlighting(string name) => DoOneTest(name);

		protected override Severity Target => Severity.ERROR;
	}
}
