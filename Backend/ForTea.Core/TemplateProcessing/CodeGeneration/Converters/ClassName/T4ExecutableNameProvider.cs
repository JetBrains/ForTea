namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName
{
	public sealed class T4ExecutableNameProvider : IT4GeneratedNameProvider
	{
		public string GeneratedClassName => T4CSharpIntermediateConverterBase.GeneratedClassNameString;
		public string GeneratedBaseClassName => T4CSharpIntermediateConverterBase.GeneratedBaseClassNameString;
		public string GeneratedBaseClassFQN => T4TextTemplatingFQNs.TextTransformation;
		public string TransformTextMethodName => T4CSharpIntermediateConverterBase.TransformTextMethodName;
	}
}
