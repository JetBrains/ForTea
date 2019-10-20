using System.Linq;
using GammaJul.ForTea.Core.Parser;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Parsing
{
	internal sealed class T4Parser : T4ParserGenerated, IParser
	{
		[NotNull]
		private ILexer OriginalLexer { get; }

		[CanBeNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		public IT4File Parse() => (IT4File) ParseFile();

		IFile IParser.ParseFile() => (IFile) ParseFile();

		public T4Parser([NotNull] ILexer lexer, [CanBeNull] IPsiSourceFile sourceFile)
		{
			OriginalLexer = lexer;
			SourceFile = sourceFile;
			SetLexer(new T4FilteringLexer(lexer));
		}

		public override TreeElement ParseFile()
		{
			var file = (File) ParseFileInternal();
			T4MissingTokenInserter.Run(file, OriginalLexer, this, null);
			// file.DocumentRangeTranslator = TODO
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
			var result = TreeElementFactory.CreateCompositeElement(ElementType.UNKNOWN_DIRECTIVE);
			var tempParsingResult = Match(T4TokenNodeTypes.DIRECTIVE_START);
			result.AppendNewChild(tempParsingResult);
			return HandleErrorInDirective(result, new UnexpectedToken("Missing directive name"));
		}

		[NotNull]
		public override TreeElement ParseIncludeDirective()
		{
			var directive = (IncludeDirective) base.ParseIncludeDirective();
			var sourceFile = SourceFile;
			if (sourceFile == null) return directive;
			var path = directive.GetPathForParsing(sourceFile).ResolvePath();
			var project = sourceFile.GetProject();
			if (project == null) return directive;
			var includeFile =
				T4ParsingContextGuardHelper.ExecuteGuarded(path, directive.Once, () => GetProjectFile(project, path));
			if (includeFile == null) return directive;
			var includedSourceFile = includeFile.ToSourceFile();
			if (includedSourceFile == null) return directive;
			var subTree = (File) includedSourceFile.BuildT4Tree();
			directive.AppendNewChild(subTree);
			return directive;
		}

		[CanBeNull]
		private static IProjectFile GetProjectFile([NotNull] IProject project, [NotNull] FileSystemPath path) =>
			project.GetSolution().FindProjectItemsByLocation(path).OfType<IProjectFile>().FirstOrDefault();
	}
}
