using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
	[TestFixture, TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4TooltipTest : IdentifierTooltipTestBase
	{
		protected override string RelativeTestDataPath => @"Highlighting\Tooltip";

		[Test]
		public void TestTransformTextTooltip() => DoNamedTest2();
	}
}
