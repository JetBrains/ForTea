using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators
{
	/// <summary>
	/// This class preprocesses T4 file
	/// to produce C# file that can be compiled and run correctly.
	/// </summary>
	public class T4CSharpPreprocessedCodeGenerator : T4CSharpCodeGeneratorBase
	{
		/// <summary>
		/// In generated code-behind, we use root file to provide intelligent support for indirectly included files,
		/// i.e. files that are included alongside with the current file into somewhere else.
		/// When generating code to be executed or placed into the project,
		/// there's no need to keep track of the context.
		/// Since the context is part of the primary PSI,
		/// we have to re-create the PSI from scratch
		/// </summary>
		protected override IT4File File => BuildT4Tree(base.File.GetSourceFile().NotNull());

		public T4CSharpPreprocessedCodeGenerator(
			[NotNull] IT4File actualFile,
			[NotNull] ISolution solution
		) : base(actualFile) => Collector = new T4CSharpCodeGenerationInfoCollector(actualFile, solution);

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpIntermediateConverter(intermediateResult, ActualFile);
	}
}
