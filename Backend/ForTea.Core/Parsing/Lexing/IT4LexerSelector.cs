using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ForTea.Core.Parsing.Lexing
{
	public interface IT4LexerSelector
	{
		[NotNull]
		ILexer SelectLexer([NotNull] IPsiSourceFile file);

		bool HasCustomLexer([NotNull] IPsiSourceFile file);
	}
}
