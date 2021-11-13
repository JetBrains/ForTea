using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests
{
	[TestFileExtension(T4FileExtensions.MainExtension)]
	[Category("Code Completion")]
	public sealed class T4CodeCompletionTest : CodeCompletionTestBase
	{
		protected override CodeCompletionTestType TestType => CodeCompletionTestType.List;
		protected override string RelativeTestDataPath => @"CodeCompletion";

		[TestCase("Directive")]
		[TestCase("Attribute")]
		[TestCase("AttributeValue")]
		[TestCase("CSharp")]
		[TestCase("VB")]
		public void TestCompletion(string name) => DoOneTest(name);
	}
}
