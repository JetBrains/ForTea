using System.IO;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Test;
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
			const string text = "<#@ template language=\"C#\"#>\n<#@ output extension=\"cs\" #>\n";
			var parser = T4ParserExposer.Create(text, new T4MockIncludeParser());
			var file = parser.ParseFile();
			file.NotNull("Could not build a PSI tree");
			string mainPsi = PsiToString(file);
			var clone = new T4CloningParser(new T4MockPsiFileProvider(file), null, T4DocumentLexerSelector.Instance).ParseFile();
			string clonePsi = PsiToString(clone);
			Assert.AreEqual(mainPsi, clonePsi, "Cloned PSI is different from original PSI");
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
