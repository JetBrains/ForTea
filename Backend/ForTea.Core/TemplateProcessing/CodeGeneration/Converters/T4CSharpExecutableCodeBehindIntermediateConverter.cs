using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpExecutableCodeBehindIntermediateConverter : T4CSharpCodeBehindIntermediateConverterBase
	{
		public T4CSharpExecutableCodeBehindIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		) : base(
			intermediateResult, file)
		{
		}

		protected override string GeneratedClassName => GeneratedClassNameString;
		protected override string GeneratedBaseClassName => GeneratedBaseClassNameString;
		protected override string GeneratedBaseClassFQN => T4TextTemplatingFQNs.TextTransformation;

		// we reference JetBrains.TextTemplating, which already contains the definition for TextTransformation
		protected override void AppendClasses() => AppendClass();
	}
}
