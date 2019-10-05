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
		[NotNull]
		private ILexer OriginalLexer { get; }

		[NotNull]
		public IT4File Parse() => (IT4File) ParseFile();

		IFile IParser.ParseFile() => (IFile) ParseFile();

		public T4Parser([NotNull] ILexer lexer)
		{
			OriginalLexer = lexer;
			SetLexer(new T4FilteringLexer(lexer));
		}

		public override TreeElement ParseFile()
		{
			// Since the included files are not part of PSI,
			// the default range translator will do
			var file = ParseFileInternal();
			T4MissingTokenInserter.Run(file, OriginalLexer, this, null);
			return file;
		}

		public override TreeElement ParseDirective()
		{
			var lookahead = myLexer.LookaheadToken(1);
			if (lookahead == T4TokenNodeTypes.TEMPLATE) return ParseTemplateDirective();
			if (lookahead == T4TokenNodeTypes.PARAMETER) return ParseParameterDirective();
			if (lookahead == T4TokenNodeTypes.OUTPUT) return ParseOutputDirective();
			if (lookahead == T4TokenNodeTypes.ASSEMBLY) return ParseAssemblyDirective();
			if (lookahead == T4TokenNodeTypes.IMPORT) return ParseImportDirective();
			if (lookahead == T4TokenNodeTypes.INCLUDE) return ParseIncludeDirective();
			if (lookahead == T4TokenNodeTypes.CLEANUP_BEHAVIOR) return ParseCleanupBehaviorDirective();
			if (lookahead == T4TokenNodeTypes.UNKNOWN_DIRECTIVE_NAME) return ParseUnknownDirective();
			// Failure
			var result = TreeElementFactory.CreateCompositeElement(Tree.Impl.ElementType.UNKNOWN_DIRECTIVE);
			var tempParsingResult = Match(T4TokenNodeTypes.DIRECTIVE_START);
			result.AppendNewChild(tempParsingResult);
			return HandleErrorInDirective(result, new UnexpectedToken("Missing directive name"));
		}
	}
}
