using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Utils
{
	public static class T4ParsingUtils
	{
		[NotNull]
		public static IT4File BuildT4Tree([NotNull] this IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService().NotNull();
			var lexer = (T4Lexer) languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			var file = new T4Parser(lexer).Parse();
			file.SetSourceFile(target);
			return file;
		}
	}
}
