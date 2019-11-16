using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Parsing
{
	public static class T4ParsingUtils
	{
		/// <note>
		/// This method builds PSI from scratch,
		/// which might cause creepy StackOverflowExceptions,
		/// difficult-to-catch bugs and performance issues!
		/// Use it VERY carefully!
		/// </note>
		[NotNull]
		public static IT4File BuildT4Tree([NotNull] this IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService().NotNull();
			var lexer = languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			return (IT4File) new T4Parser(lexer, target, target).ParseFile();
		}
	}
}
