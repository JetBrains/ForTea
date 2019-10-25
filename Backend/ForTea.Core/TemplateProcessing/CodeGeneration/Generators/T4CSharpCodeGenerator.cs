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
	public class T4CSharpCodeGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeGenerator(
			[NotNull] IT4File actualFile,
			[NotNull] ISolution solution
		) : base(actualFile) => Collector = new T4CSharpCodeGenerationInfoCollector(actualFile, solution);

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpIntermediateConverter(intermediateResult, ActualFile);
	}
}