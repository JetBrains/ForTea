using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Generators
{
	public sealed class T4CSharpExecutableCodeGenerator : T4CSharpCodeGenerator
	{
		public T4CSharpExecutableCodeGenerator(
			[NotNull] IT4File actualFile,
			[NotNull] ISolution solution
		) : base(actualFile, solution)
		{
		}

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		)
		{
			var referenceExtractionManager = ActualFile.GetSolution().GetComponent<IT4ReferenceExtractionManager>();
			return new T4CSharpExecutableIntermediateConverter(intermediateResult, ActualFile, referenceExtractionManager);
		}
	}
}
