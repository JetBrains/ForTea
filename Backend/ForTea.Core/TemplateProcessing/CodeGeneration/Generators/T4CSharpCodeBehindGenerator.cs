using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
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

		public T4CSharpCodeBehindGenerator(
			[NotNull] IT4File file,
			[NotNull] ISolution solution
		) : base(file) => Solution = solution;

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector =>
			new T4CSharpCodeBehindGenerationInfoCollector(File, Solution);

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		)
		{
			var rootTemplateKindProvider = Solution.GetComponent<IT4RootTemplateKindProvider>();
			var projectFile = File.PhysicalPsiSourceFile.ToProjectFile().NotNull();
			if (rootTemplateKindProvider.IsRootPreprocessedTemplate(projectFile))
				return new T4CSharpPreprocessedCodeBehindIntermediateConverter(intermediateResult, File);
			return new T4CSharpExecutableCodeBehindIntermediateConverter(intermediateResult, File);
		}
	}
}
