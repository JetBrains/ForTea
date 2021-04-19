using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser.Impl
{
	public sealed class T4IncludeParser : IT4IncludeParser
	{
		[CanBeNull]
		private IPsiSourceFile LogicalSourceFile { get; }

		[CanBeNull]
		private IPsiSourceFile PhysicalSourceFile { get; }

		[CanBeNull]
		private IT4IncludeResolver IncludeResolver { get; }

		[NotNull]
		private IT4LexerSelector LexerSelector { get; }

		[CanBeNull]
		private T4MacroResolveContext Context { get; }

		public T4IncludeParser(
			[CanBeNull] IPsiSourceFile logicalSourceFile,
			[CanBeNull] IPsiSourceFile physicalSourceFile,
			[CanBeNull] IT4IncludeResolver includeResolver,
			[NotNull] IT4LexerSelector lexerSelector,
			[CanBeNull] T4MacroResolveContext context = null
		)
		{
			LogicalSourceFile = logicalSourceFile;
			PhysicalSourceFile = physicalSourceFile;
			IncludeResolver = includeResolver;
			LexerSelector = lexerSelector;
			Context = context;
		}

		public ITreeNode Parse(IT4IncludeDirective directive)
		{
			if (LogicalSourceFile == null) return null;
			var pathWithMacros = directive.ResolvedPath;
			var path = IncludeResolver?.ResolvePath(pathWithMacros);
			if (path == null) return null;
			var includeFile = T4ParsingContextHelper.ExecuteGuarded(
				path,
				directive.Once,
				() => IncludeResolver?.Resolve(pathWithMacros)
			);
			if (includeFile == null) return null;
			return BuildIncludedT4Tree(includeFile);
		}

		[NotNull]
		internal CompositeElement BuildIncludedT4Tree([NotNull] IPsiSourceFile target)
		{
			var lexer = LexerSelector.SelectLexer(target);
			// We need to parse File here, not IncludedFile,
			// because File makes some important error recovery and token reinsertion
			var parser = new T4Parser(lexer, target, PhysicalSourceFile, LexerSelector, Context);
			return IncludedFile.FromOtherNode(parser.ParseFileWithoutCleanup());
		}
	}
}
