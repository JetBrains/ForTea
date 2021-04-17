using System.Collections.Generic;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Psi.Cache;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Service {

	/// <summary>Base, file independent language service for T4.</summary>
	[Language(typeof(T4Language))]
	public sealed class T4LanguageService : LanguageService {
		[NotNull]
		private ILogger Logger { get; }

		/// <summary>Creates a lexer that filters tokens that have no meaning.</summary>
		/// <param name="lexer">The base lexer.</param>
		/// <returns>An implementation of a filtering lexer.</returns>
		public override ILexer CreateFilteringLexer(ILexer lexer)
			=> new T4FilteringLexer(lexer);

		/// <summary>Gets a factory capable of creating T4 lexers.</summary>
		/// <returns>An implementation of <see cref="ILexerFactory"/>.</returns>
		public override ILexerFactory GetPrimaryLexerFactory()
			=> T4LexerFactory.Instance;

		/// <summary>Creates a parser for a given PSI source file.</summary>
		/// <param name="lexer">The lexer that the parser will use.</param>
		/// <param name="module">The module owning the source file.</param>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>A T4 parser that operates onto <paramref name="lexer"/>.</returns>
		/// <note>
		/// For the sake of providing the most complete intelligent support,
		/// we make the widest possible context part of the primary PSI.
		/// </note>
		[NotNull]
		public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile)
		{
			if (sourceFile == null)
			{
				Logger.Warn("Creating parser for null sourceFile");
				return new T4Parser(lexer, null, null, T4DocumentLexerSelector.Instance);
			}

			var solution = sourceFile.GetSolution();
			var graph = solution.GetComponent<IT4FileDependencyGraph>();
			var rootSourceFile = graph.FindBestRoot(sourceFile).NotNull();
			var selector = new T4DelegatingLexerSelector(lexer, sourceFile, T4DocumentLexerSelector.Instance);
			var rootLexer = selector.SelectLexer(rootSourceFile);
			if (rootSourceFile == sourceFile) return new T4Parser(rootLexer, rootSourceFile, sourceFile, selector);
			return new T4CloningParser(rootSourceFile, selector);
		}

		/// <summary>
		/// Gets a cache provider for T4 files.
		/// TODO: implement a cache provider
		/// </summary>
		public override ILanguageCacheProvider CacheProvider
			=> null;

		public override bool SupportTypeMemberCache
			=> false;

		/// <summary>Gets a type presenter.</summary>
		public override ITypePresenter TypePresenter
			=> DefaultTypePresenter.Instance;

		public override bool IsCaseSensitive
			=> true;

		public override IEnumerable<ITypeDeclaration> FindTypeDeclarations(IFile file)
			=> EmptyList<ITypeDeclaration>.InstanceList;

		public T4LanguageService(
			[NotNull] T4Language t4Language,
			[NotNull] IConstantValueService constantValueService,
			[NotNull] ILogger logger
		) : base(t4Language, constantValueService) => Logger = logger;
	}

}