using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ForTea.Core.Test
{
	/// <summary>
	/// This class exists in order to make the internal T4 parser visible for tests
	/// </summary>
	public static class T4ParserExposer
	{
		[NotNull]
		public static IParser Create([NotNull] string text, [CanBeNull] IT4IncludeParser includeParser, [CanBeNull] IPsiSourceFile sourceFile = null)
		{
			var buffer = new StringBuffer(text);
			var lexer = new T4Lexer(buffer);
			return new T4Parser(lexer, sourceFile, null, T4DocumentLexerSelector.Instance, includeParser: includeParser);
		}

		[CanBeNull]
		public static IT4File ParseFileWithoutCleanup(this IParser parser)
		{
			if (parser is not T4Parser t4Parser) return null;
			return t4Parser.ParseFileWithoutCleanup();
		}
	}
}
