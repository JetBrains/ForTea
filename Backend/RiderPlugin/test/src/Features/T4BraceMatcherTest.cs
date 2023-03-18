using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.ReSharper.FeaturesTestFramework.ContextHighlighters;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features
{
  [Category("T4"), Category("MatchingBraces")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public class T4BraceMatcherTest : ContextHighlighterTestBase
  {
    protected override string ExtraPath => @"BracesMatcher";

    [TestCase("T4BlockEndMatching")]
    [TestCase("CSharpBraceMatching")]
    [TestCase("Macro")]
    [TestCase("EnvironmentVariable")]
    [TestCase("DirectiveAttribute")]
    public void TestBraceMatcher([NotNull] string name) => DoOneTest(name);
  }
}