using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features.TypingAssist.Macro
{
  [TestFixture]
  [Category("Typing assist")]
  [Category("T4")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public class T4MacroTypingAssistTest : T4MacroTestBase
  {
    [TestCase("DollarInPath")]
    [TestCase("DollarInText")]
    [TestCase("DollarInDirective")]
    [TestCase("DollarInCSharp")]
    public void TestTypingAssist([NotNull] string name) => DoOneTest(name);

    protected override string StringToType => "$";
  }
}