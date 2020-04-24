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
	public sealed class T4CSharpPreprocessedCodeGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpPreprocessedCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] ISolution solution
		) : base(file) => Collector = new T4CSharpCodeGenerationInfoCollector(file, solution);

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }
		protected override T4CSharpIntermediateConverterBase Converter => new T4CSharpIntermediateConverter(File);
	}
}
