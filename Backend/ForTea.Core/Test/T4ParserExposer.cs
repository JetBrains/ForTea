using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using JetBrains.Annotations;
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
		public static IParser Create([NotNull] string text, [CanBeNull] IT4IncludeParser includeParser)
		{
			var buffer = new StringBuffer(text);
			var lexer = new T4Lexer(buffer);
			return new T4Parser(lexer, null, null, T4DocumentLexerSelector.Instance, includeParser: includeParser);
		}
	}
}
