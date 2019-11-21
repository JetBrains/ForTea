using System;
using System.Linq;
using GammaJul.ForTea.Core.Parser;
using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing
{
	internal sealed class T4Parser : T4ParserGenerated, IParser
	{
		[NotNull]
		private ILexer OriginalLexer { get; }

		[CanBeNull]
		private IPsiSourceFile LogicalSourceFile { get; }

		[CanBeNull]
		private IPsiSourceFile PhysicalSourceFile { get; }

		[NotNull]
		private T4MacroResolveContext Context { get; }

		/// <note>
		/// Since the other ParseFile method is used in some external places,
		/// this method should not contain any additional logic
		/// </note>
		[NotNull]
		IFile IParser.ParseFile() => (IFile) ParseFile();

		public T4Parser(
			[NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile logicalSourceFile,
			[CanBeNull] IPsiSourceFile physicalSourceFile,
			[CanBeNull] T4MacroResolveContext context = null
		)
		{
			OriginalLexer = lexer;
			LogicalSourceFile = logicalSourceFile;
			PhysicalSourceFile = physicalSourceFile;
			Context = context ?? new T4MacroResolveContext();
			SetLexer(new T4FilteringLexer(lexer));
		}

		[NotNull]
		public override TreeElement ParseFile()
		{
			var result = ParseFileWithoutCleanup();
			result.SetSourceFile(PhysicalSourceFile);
			T4ParsingContextHelper.Reset();
			return result;
		}

		private static void SetUpRangeTranslators([NotNull] File file)
		{
			file.DocumentRangeTranslator = new T4DocumentRangeTranslator(file);
			foreach (var include in file.Includes.Cast<IncludedFile>())
			{
				SetUpRangeTranslators(include);
			}
		}

		private static void SetUpRangeTranslators([NotNull] IncludedFile file)
		{
			file.DocumentRangeTranslator = new T4DocumentRangeTranslator(file);
			foreach (var include in file.IncludedFilesEnumerable.Cast<IncludedFile>())
			{
				SetUpRangeTranslators(include);
			}
		}

		[NotNull]
		private File ParseFileWithoutCleanup()
		{
			using (Context.RegisterNextLayer(LogicalSourceFile.ToProjectFile()))
			{
				return T4ParsingContextHelper.ExecuteGuarded(
					LogicalSourceFile.GetLocation(),
					false,
					() =>
					{
						var file = (File) ParseFileInternal();
						T4MissingTokenInserter.Run(file, OriginalLexer, this, null);
						if (LogicalSourceFile != null) file.LogicalPsiSourceFile = LogicalSourceFile;
						SetUpResolveContexts(file);
						ResolveIncludes(file);
						SetUpRangeTranslators(file);
						return file;
					}
				).NotNull("Attempted to parse same file recursively twice");
			}
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

		private void SetUpResolveContexts([NotNull] IT4File file)
		{
			foreach (var directive in file.BlocksEnumerable.OfType<IT4DirectiveWithPath>())
			{
				directive.ResolutionContext = Context.MostSuitableProjectFile;
			}
		}

		private void ResolveIncludes([NotNull] IT4File file)
		{
			foreach (var includeDirective in file.Blocks.OfType<IncludeDirective>())
			{
				var includedFile = ResolveIncludeDirective(includeDirective);
				if (includedFile == null) continue;
				includeDirective.parent.AddChildAfter(includedFile, includeDirective);
			}
		}

		[CanBeNull]
		private CompositeElement ResolveIncludeDirective([NotNull] IncludeDirective directive)
		{
			var sourceFile = LogicalSourceFile;
			if (sourceFile == null) return null;
			var pathWithMacros = directive.GetPathForParsing(sourceFile);
			var path = pathWithMacros.ResolvePath();
			var project = sourceFile.GetProject();
			if (project == null) return null;
			var includeFile =
				T4ParsingContextHelper.ExecuteGuarded(path, directive.Once, () => pathWithMacros.Resolve());
			if (includeFile == null) return null;
			return BuildIncludedT4Tree(includeFile);
		}

		[NotNull]
		private CompositeElement BuildIncludedT4Tree([NotNull] IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService().NotNull();
			var lexer = languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			// We need to parse File here, not IncludedFile,
			// because File makes some important error recovery and token reinsertion
			return IncludedFile.FromOtherNode(new T4Parser(lexer, target, PhysicalSourceFile, Context)
				.ParseFileWithoutCleanup());
		}

		[Obsolete("This method should never be called", true)]
		public override TreeElement ParseIncludedFile() => throw new InvalidOperationException();
	}
}
