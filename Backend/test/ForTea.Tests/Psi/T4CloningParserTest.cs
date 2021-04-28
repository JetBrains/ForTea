using System.IO;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Test;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ForTea.Tests.Mock;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	public sealed class T4CloningParserTest
	{
		[Test]
		public void TestCorrectness()
		{
			const string text = "<#@ template language=\"C#\" #>\n<#@ output extension=\"cs\" #>\n";
			var parser = T4ParserExposer.Create(text, new T4MockIncludeParser());
			var file = parser.ParseFile();
			file.NotNull("Could not build a PSI tree");
			string mainPsi = PsiToString(file);

			var clone = new T4CloningParser(new T4MockPsiFileProvider(file), null, T4DocumentLexerSelector.Instance).ParseFile();
			string clonePsi = PsiToString(clone);
			Assert.AreEqual(mainPsi, clonePsi, "Cloned PSI is different from original PSI");
		}

		[Test]
		public void TestCorrectnessWithInclude()
		{
			const string includeText = "<#@ using namespace=\"System.Text\" #>\n<#+\n    public void Foo()\n    {\n    }\n#>";
			const string includerText = "<#@ template language=\"C#\" #>\n<#@ output extension=\"cs\" #>\nSome random text<#@ include file=\"dummy.ttinclude\" #>More random text\nEven more random text";

			var includePsi = T4ParserExposer.Create(includeText, new T4MockIncludeParser()).ParseFile();
			var includerPsi = T4ParserExposer.Create(includerText, new T4MockIncludeParser(includePsi)).ParseFile();
			string includerPsiText = PsiToString(includerPsi.NotNull());
			T4CloningParserTestUtils.InitializeResolvePaths((IT4File) includerPsi);

			var psiProvider = new T4MockPsiFileProvider(includerPsi).NotNull();
			var clonePsi = new T4CloningParser(psiProvider, null, T4DocumentLexerSelector.Instance).ParseFile();
			string clonePsiText = PsiToString(clonePsi);

			Assert.AreEqual(includerPsiText, clonePsiText, "Cloned PSI is different from original PSI");
		}

		[NotNull]
		private static string PsiToString([NotNull] IFile file)
		{
			var writer = new StringWriter();
			DebugUtil.DumpPsi(writer, file);
			string result = writer.GetStringBuilder().ToString();
			return result;
		}
	}
}
