using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Converters;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Generators
{
	public sealed class T4CSharpExecutableCodeGenerator : T4CSharpCodeGenerator
	{
		public T4CSharpExecutableCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file, manager)
		{
		}

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpExecutableIntermediateConverter(intermediateResult, File);
	}
}