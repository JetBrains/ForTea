using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Text;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4LexerTest : LexerTestBase
	{
		protected override string RelativeTestDataPath => @"Psi\Lexer";
		protected override ILexer CreateLexer(IBuffer buffer) => new T4Lexer(buffer);

		[TestCase("Simple")]
		[TestCase("CSharpCode")]
		[TestCase("VBCode")]
		[TestCase("ComplexValue")]
		[TestCase("EmptyDirective")]
		[TestCase("MultilineExpression")]
		public void TestLexer(string name) => DoOneTest(name);

		[TestCase("ForgottenBlockEnd")]
		[TestCase("ForgottenBlockEnd2")]
		public void TestErrorRecovery(string name) => DoOneTest(name);
	}
}
