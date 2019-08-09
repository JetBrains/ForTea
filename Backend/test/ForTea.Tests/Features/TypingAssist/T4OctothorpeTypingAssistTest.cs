using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.FeaturesTestFramework.TypingAssist;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features.TypingAssist
{
	[TestFixture]
	[Category("Typing assist")]
	[Category("T4")]
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public class T4OctothorpeTypingAssistTest : TypingAssistTestBase
	{
		protected override string RelativeTestDataPath => @"Features\TypingAssist";

		[TestCase("BlockEndTag")]
		[TestCase("BlockStartTag")]
		public void TestTypingAssist([NotNull] string name) => DoOneTest(name);

		protected override void DoAdditionalTyping(ITextControl textControl, TestOptionsIterator.TestData data) =>
			textControl.EmulateTyping('#');
	}
}
