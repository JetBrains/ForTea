using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName
{
	public sealed class T4PreprocessedNameProvider : IT4GeneratedNameProvider
	{
		public T4PreprocessedNameProvider([NotNull] IPsiSourceFile file, [NotNull] string transformTextSuffix)
		{
			File = file;
			TransformTextMethodName = T4CSharpIntermediateConverterBase.TransformTextMethodName + transformTextSuffix;
		}

		[NotNull]
		private IPsiSourceFile File { get; }

		public string GeneratedClassName => File.CreateGeneratedClassName();
		public string GeneratedBaseClassName => GeneratedClassName + "Base";
		public string GeneratedBaseClassFQN => GeneratedBaseClassName;
		public string TransformTextMethodName { get; }
	}
}
