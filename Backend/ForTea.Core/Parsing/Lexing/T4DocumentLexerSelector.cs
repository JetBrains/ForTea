using GammaJul.ForTea.Core.Psi;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ForTea.Core.Parsing.Lexing
{
	public sealed class T4DocumentLexerSelector : IT4LexerSelector
	{
		private T4DocumentLexerSelector()
		{
		}

		public static IT4LexerSelector Instance { get; } = new T4DocumentLexerSelector();

		public ILexer SelectLexer(IPsiSourceFile file) => T4Language
			.Instance
			.LanguageService()
			.NotNull()
			.GetPrimaryLexerFactory()
			.CreateLexer(file.Document.Buffer);

		public bool HasCustomLexer(IPsiSourceFile file) => false;
	}
}
