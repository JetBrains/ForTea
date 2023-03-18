using GammaJul.ForTea.Core.Daemon.QuickFixes;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting.QuickFix
{
  [TestFixture]
  [Category("T4")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public sealed class T4ReplaceWithFeatureBlockQuickFixTest : QuickFixTestBase<T4ReplaceWithFeatureBlockQuickFix>
  {
    protected override string RelativeTestDataPath => @"Highlighting\QuickFix\ReplaceWithFeatureBlock";

    [Test]
    public void Test01() => DoNamedTest2();

    [Test]
    public void Test02() => DoNamedTest2();
  }
}