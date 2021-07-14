using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
	[Ignore("Highlighting is delegated to frontend")]
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4CSharpHighlightingTest : HighlightingTestBase
	{
		protected override PsiLanguageType CompilerIdsLanguage => T4Language.Instance;
		protected override string RelativeTestDataPath => @"Highlighting\CSharp";

		protected override bool ColorIdentifiers => true;
		protected override bool InplaceUsageAnalysis => true;

		protected override bool HighlightingPredicate(
			IHighlighting highlighting,
			IPsiSourceFile sourceFile,
			IContextBoundSettingsStore settingsStore
		) => true;

		[TestCase("CSharp")]
		[TestCase("VB")]
		public void TestHighlighting(string name) => DoOneTest(name);
	}
}
