using GammaJul.ForTea.Core.Daemon.QuickFixes.Moving;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting.QuickFix
{
  [TestFixture]
  [Category("T4")]
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public sealed class T4PlaceStatementBeforeFeatureQuickFixTest :
    QuickFixTestBase<T4PlaceStatementBeforeFeatureQuickFix>
  {
    protected override string RelativeTestDataPath => @"Highlighting\QuickFix\PlaceStatementBeforeFeature";

    [Test]
    public void Test01() => DoNamedTest2();
  }
}