using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4AssemblyReferenceTest : T4HighlightingTestBase
	{
		protected override string RelativeTestDataPath => @"Highlighting\Assembly";

		[TestCase("NoReference")]
		[TestCase("ReferenceWithNoImport")]
		[TestCase("NoReferenceWithImport")]
		[TestCase("ReferenceWithImport")]
		[Ignore("Test framework doesn't copy any files to test folder")]
		public void TestAssemblyReference(string name) => DoTestSolution(name + "", "Foo.dll");

		protected override Severity Target => Severity.ERROR;
	}
}
