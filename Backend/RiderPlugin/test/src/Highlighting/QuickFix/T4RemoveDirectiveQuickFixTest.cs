using GammaJul.ForTea.Core.Daemon.QuickFixes.Removing;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting.QuickFix
{
  [TestFixture]
  [Category("T4")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public sealed class T4RemoveDirectiveQuickFixTest : QuickFixTestBase<T4RemoveDirectiveQuickFix>
  {
    protected override string RelativeTestDataPath => @"Highlighting\QuickFix\RemoveDirective";

    [Test]
    public void TestDuplicateTemplateDirective() => DoNamedTest2();

    [Test]
    public void TestDuplicateOutputDirective() => DoNamedTest2();

    [Test, Ignore("No way to make files preprocessed in tests")]
    public void TestDirectiveInPreprocessedFile() => DoNamedTest2();
  }
}