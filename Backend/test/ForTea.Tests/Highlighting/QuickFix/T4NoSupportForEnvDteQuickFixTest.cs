using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ForTea.RiderPlugin.Daemon.QuickFixes;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting.QuickFix
{
	[TestFixture]
	[Category("T4")]
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4NoSupportForEnvDteQuickFixTest : QuickFixTestBase<T4RemoveEnvDteReferenceQuickFix>
	{
		protected override string RelativeTestDataPath => @"Highlighting\QuickFix\RemoveEnvDteReference";
		[Test] public void Test01() => DoNamedTest2();
	}
}
