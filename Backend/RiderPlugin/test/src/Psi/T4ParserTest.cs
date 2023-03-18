using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public sealed class T4ParserTest : ParserTestBase<T4Language>
  {
    protected override string RelativeTestDataPath => @"Psi\Parser";

    [TestCase("Simple")]
    [TestCase("SimpleText")]
    [TestCase("CSharpCode")]
    [TestCase("CaseInsensivity")]
    [TestCase("Macros")]
    [TestCase("EmptyDirective")]
    [TestCase("EmptyExpressionBlock")]
    [TestCase("LonelyBlockEnd")]
    [TestCase("BlockStartInAttributeValue")]
    public void TestParser(string name) => DoOneTest(name);

    [TestCase("ForgottenBlockEnd")]
    [TestCase("ForgottenBlockEnd2")]
    [TestCase("ForgottenEndQuote")]
    [TestCase("ForgottenStartQuote")]
    [TestCase("ForgottenQuotes")]
    public void TestErrorRecovery(string name) => DoOneTest(name);
  }
}