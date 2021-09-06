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
	public sealed class T4RemoveRedundantAssemblyDirectiveQuickFixTest :
		QuickFixTestBase<T4RemoveRedundantAssemblyDirectiveQuickFix>
	{
		protected override string RelativeTestDataPath => @"Highlighting\QuickFix\RemoveRedundantAssemblyDirective";
		[Test] public void Test01() => DoNamedTest2();
	}
}
