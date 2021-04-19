using System;
using GammaJul.ForTea.Core.Parser;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser.Include;
using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.Parsing.Token;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser
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

		[NotNull]
		private T4MacroInitializer MacroInitializer { get; }

		[NotNull]
		private T4RangeTranslatorInitializer RangeTranslatorInitializer { get; }

		[NotNull]
		private IT4IncludeParser IncludeParser { get; }

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
			[NotNull] IT4LexerSelector lexerSelector,
			[CanBeNull] T4MacroResolveContext context = null
		)
		{
			OriginalLexer = lexer;
			LogicalSourceFile = logicalSourceFile;
			PhysicalSourceFile = physicalSourceFile;
			Context = context ?? new T4MacroResolveContext();
			SetLexer(new T4FilteringLexer(lexer));
			var solution = physicalSourceFile?.GetSolution();
			var includeResolver = solution?.GetComponent<IT4IncludeResolver>();
			var macroResolver = solution?.GetComponent<IT4MacroResolver>();
			MacroInitializer = new T4MacroInitializer(LogicalSourceFile, Context, macroResolver);
			RangeTranslatorInitializer = new T4RangeTranslatorInitializer();
			IncludeParser = new T4IncludeParser(logicalSourceFile, physicalSourceFile, includeResolver, lexerSelector, Context);
		}

		[NotNull]
		public override TreeElement ParseFile()
		{
			var result = ParseFileWithoutCleanup();
			RangeTranslatorInitializer.SetUpRangeTranslators(result);
			result.SetSourceFile(PhysicalSourceFile);
			T4ParsingContextHelper.Reset();
			return result;
		}

		[NotNull]
		internal File ParseFileWithoutCleanup()
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
						if (!MacroInitializer.CanResolveMacros) return file;
						MacroInitializer.ResolveMacros(file);
						ParseIncludes(file);
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

		private void ParseIncludes([NotNull] IT4File file)
		{
			foreach (var includeDirective in file.Blocks.OfType<IncludeDirective>())
			{
				var includedFile = IncludeParser.Parse(includeDirective);
				if (includedFile == null) continue;
				includeDirective.parent.AddChildAfter(includedFile, includeDirective);
			}
		}

		#pragma warning disable CS0809
		[Obsolete("This method should never be called", true)]
		public override TreeElement ParseIncludedFile() => throw new InvalidOperationException();
		#pragma warning restore CS0809
	}
}
