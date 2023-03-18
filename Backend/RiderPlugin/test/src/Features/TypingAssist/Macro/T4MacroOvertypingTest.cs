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
  public sealed class T4MacroOvertypingTest : T4MacroTestBase
  {
    [TestCase("Overtyping")]
    public void TestTypingAssist([NotNull] string name) => DoOneTest(name);

    protected override string StringToType => ")";
  }
}