using GammaJul.ForTea.Core.Psi.FileType;
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
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4QuoteTypingAssistTest : TypingAssistTestBase
	{
		protected override string RelativeTestDataPath => @"Features\TypingAssist\Quote";
		private int QuoteNumber { get; set; }

		[TestCase("OneQuoteInCSharp", 1)]
		[TestCase("TwoQuotesInCSharp", 2)]
		[TestCase("ThreeQuotesInCSharp", 3)]
		[TestCase("OneQuoteInDirective", 1)]
		[TestCase("TwoQuotesInDirective", 2)]
		[TestCase("ThreeQuotesInDirective", 3)]
		[TestCase("OneQuoteInText", 1)]
		[TestCase("TwoQuotesInText", 2)]
		[TestCase("ThreeQuotesInText", 3)]
		[TestCase("SurroundWithQuotesInCSharp", 1)]
		[TestCase("SurroundWithQuotesInCSharp2", 1)]
		public void TestTypingAssist([NotNull] string name, int quoteNumber)
		{
			QuoteNumber = quoteNumber;
			DoOneTest(name);
		}

		protected override void DoAdditionalTyping(ITextControl textControl, TestOptionsIterator.TestData data)
		{
			for (int index = 0; index < QuoteNumber; index += 1)
			{
				textControl.EmulateTyping('"');
			}
		}
	}
}
