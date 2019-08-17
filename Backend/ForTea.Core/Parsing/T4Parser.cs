using GammaJul.ForTea.Core.Parser;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing
{
	internal sealed class T4Parser : T4ParserGenerated, IParser
	{
		public IT4File Parse() => (IT4File) ParseT4File();

		public IFile ParseFile() => (IFile) ParseT4File();

		// TODO: is this line necessary?
		public T4Parser([NotNull] ILexer lexer) => SetLexer(lexer);

		public override TreeElement ParseT4Directive()
		{
			var directiveStart = myLexer.TokenType;
			if (directiveStart != T4TokenNodeTypes.DIRECTIVE_START)
				throw new UnexpectedToken(ErrorMessages.GetErrorMessage2());

			return SelectAndParseT4DirectiveInternal();
		}

		private TreeElement SelectAndParseT4DirectiveInternal()
		{
			var lookahead = myLexer.LookaheadTokenSkipping(1, T4TokenNodeTypes.WHITE_SPACE);
			if (lookahead == T4TokenNodeTypes.TEMPLATE) return ParseT4TemplateDirective();
			if (lookahead == T4TokenNodeTypes.PARAMETER) return ParseT4ParameterDirective();
			if (lookahead == T4TokenNodeTypes.OUTPUT) return ParseT4OutputDirective();
			if (lookahead == T4TokenNodeTypes.ASSEMBLY) return ParseT4AssemblyDirective();
			if (lookahead == T4TokenNodeTypes.IMPORT) return ParseT4ImportDirective();
			if (lookahead == T4TokenNodeTypes.INCLUDE) return ParseT4IncludeDirective();
			if (lookahead == T4TokenNodeTypes.CLEANUP_BEHAVIOR) return ParseT4CleanupBehaviorDirective();
			if (lookahead == T4TokenNodeTypes.UNKNOWN_DIRECTIVE_NAME) return ParseT4UnknownDirective();
			throw new UnexpectedToken(ErrorMessages.GetErrorMessage2());
		}
	}
}
