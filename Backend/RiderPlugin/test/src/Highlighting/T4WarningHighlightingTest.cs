using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public class T4WarningHighlightingTest : T4HighlightingTestBase
  {
    protected override string RelativeTestDataPath => @"Highlighting\Warning";

    [TestCase("EscapedKeyword")]
    [TestCase("DuplicateDirective")]
    [TestCase("RedundantInclude", Ignore = "Includes are not resolved in tests")]
    [TestCase("UnexpectedAttribute")]
    [TestCase("UnexpectedDirective")]
    [TestCase("EmptyDirective")]
    [TestCase("UnsupportedLanguage")]
    [TestCase("DuplicateAssemblyDirective")]
    public void TestHighlighting(string name) => DoOneTest(name);

    protected override Severity Target => Severity.WARNING;
  }
}