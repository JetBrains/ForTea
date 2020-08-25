using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName
{
	public sealed class T4PreprocessedClassNameProvider : IT4GeneratedClassNameProvider
	{
		public T4PreprocessedClassNameProvider([NotNull] IPsiSourceFile file) => File = file;

		[NotNull]
		private IPsiSourceFile File { get; }

		public string GeneratedClassName => File.CreateGeneratedClassName();
		public string GeneratedBaseClassName => GeneratedClassName + "Base";
		public string GeneratedBaseClassFQN => GeneratedBaseClassName;
	}
}
