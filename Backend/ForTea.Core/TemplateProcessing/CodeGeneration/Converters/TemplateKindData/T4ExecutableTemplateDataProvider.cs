namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.TemplateKindData
{
	public sealed class T4ExecutableTemplateDataProvider : IT4TemplateKindDependentDataProvider
	{
		public string GeneratedClassName => T4CSharpIntermediateConverterBase.GeneratedClassNameString;
		public string GeneratedBaseClassName => T4CSharpIntermediateConverterBase.GeneratedBaseClassNameString;
		public string GeneratedBaseClassFQN => T4TextTemplatingFQNs.TextTransformation;
		public string TransformTextMethodName => T4CSharpIntermediateConverterBase.TransformTextMethodName;
		public string TransformTextAttributes => "";
	}
}
