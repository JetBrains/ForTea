using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.FeaturesTestFramework.TypingAssist;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features.TypingAssist.EnvironmentVariables
{
  [TestFixture]
  [Category("Typing assist")]
  [Category("T4")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public class T4FullVariableNameTypingAssistTest : TypingAssistTestBase
  {
    protected override string RelativeTestDataPath => @"Features\TypingAssist\EnvironmentVariables";

    [Test]
    public void TestFullVariableName() => DoNamedTest2();

    protected override void DoAdditionalTyping(ITextControl textControl, TestOptionsIterator.TestData data)
    {
      textControl.EmulateTyping('%');
      textControl.EmulateTyping('U');
      textControl.EmulateTyping('s');
      textControl.EmulateTyping('e');
      textControl.EmulateTyping('r');
      textControl.EmulateTyping('N');
      textControl.EmulateTyping('a');
      textControl.EmulateTyping('m');
      textControl.EmulateTyping('e');
      textControl.EmulateTyping('%');
    }
  }
}