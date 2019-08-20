using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.FeaturesTestFramework.TypingAssist;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features.TypingAssist.EnvironmentVariables
{
	[TestFixture]
	[Category("Typing assist")]
	[Category("T4")]
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public class T4EmptyVariableNameTypingAssistTest : TypingAssistTestBase
	{
		protected override string RelativeTestDataPath => @"Features\TypingAssist\EnvironmentVariables";

		[Test]
		public void TestEmptyVariableName() => DoNamedTest2();

		protected override void DoAdditionalTyping(ITextControl textControl, TestOptionsIterator.TestData data)
		{
			textControl.EmulateTyping('%');
			textControl.EmulateTyping('%');
		}
	}
}
