using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Invalidation;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators
{
	/// <summary>
	/// This class generates a code-behind file
	/// from C# embedded statements and directives in the T4 file.
	/// That file is used for providing code highlighting and other code insights
	/// in T4 source file.
	/// That code is not intended to be compiled and run.
	/// </summary>
	internal sealed class T4CSharpCodeBehindGenerator : T4CSharpCodeGeneratorBase
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		public T4CSharpCodeBehindGenerator(
			[NotNull] IT4File actualFile,
			[NotNull] ISolution solution
		) : base(actualFile)
		{
			Solution = solution;
			Graph = solution.GetComponent<IT4FileDependencyGraph>();
		}

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector =>
			new T4CSharpCodeBehindGenerationInfoCollector(Root, Solution);

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpCodeBehindIntermediateConverter(intermediateResult, ActualFile);

		/// <summary>
		/// In generated code-behind, we use root file to provide intelligent support for indirectly included files,
		/// i.e. files that are included alongside with the current file into somewhere else.
		/// </summary>
		[NotNull]
		private IT4File Root
		{
			get
			{
				var projectFile = ActualFile.GetSourceFile().ToProjectFile().NotNull();
				var rootPsiSourceFile = Graph.FindBestRoot(projectFile).ToSourceFile();
				// Since primary and secondary PSI files are built simultaneously,
				// by this moment the primary PSI has not yet been registered in caches.
				// Therefore requesting it would cause
				// constructing a brand new primary and secondary PSI,
				// leading to StackOverflowExceptions.
				// This is why this corner case has to be dealt with separately.
				if (ActualFile.GetSourceFile() == rootPsiSourceFile) return ActualFile;
				// It is NOT safe to assume that PSI for the root file can be built
				// The primary PSI only relies on source files.
				// The secondary PSI only relies on the primary PSI of
				// either the current file, or a file that includes it.
				// In most cases it means that PSI construction
				// is well-ordered and we would be able to call rootPsiSourceFile.GetPrimaryPsi,
				// except for the edge case of recursion in includes,
				// which would cause StackOverflowExceptions
				// TODO: somehow deal with that edge case and avoid building a new tree
				return BuildT4Tree(rootPsiSourceFile.NotNull());
			}
		}

		/// <note>
		/// This method builds PSI from scratch,
		/// which might cause creepy StackOverflowExceptions,
		/// difficult-to-catch bugs and performance issues!
		/// Use it VERY carefully!
		/// </note>
		[NotNull]
		private static IT4File BuildT4Tree([NotNull] IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService().NotNull();
			var lexer = languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			var file = (IT4File) new T4Parser(lexer, target).ParseFile();
			file.SetSourceFile(target);
			return file;
		}
	}
}
