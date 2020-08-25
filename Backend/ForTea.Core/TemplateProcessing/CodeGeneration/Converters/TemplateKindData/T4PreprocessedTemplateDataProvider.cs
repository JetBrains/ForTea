using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.TemplateKindData
{
	public sealed class T4PreprocessedTemplateDataProvider : IT4TemplateKindDependentDataProvider
	{
		public T4PreprocessedTemplateDataProvider([NotNull] IPsiSourceFile file, [NotNull] string transformTextSuffix)
		{
			File = file;
			TransformTextMethodName = T4CSharpIntermediateConverterBase.TransformTextMethodName + transformTextSuffix;
			if (string.IsNullOrEmpty(transformTextSuffix))
			{
				TransformTextAttributes = "";
			}
			else
			{
				TransformTextAttributes = "[__ReSharperSynthetic]";
			}
		}

		[NotNull]
		private IPsiSourceFile File { get; }

		public string GeneratedClassName => File.CreateGeneratedClassName();
		public string GeneratedBaseClassName => GeneratedClassName + "Base";
		public string GeneratedBaseClassFQN => GeneratedBaseClassName;
		public string TransformTextMethodName { get; }
		public string TransformTextAttributes { get; }
	}
}
