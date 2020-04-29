using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName
{
	public sealed class T4PreprocessedClassNameProvider : IT4GeneratedClassNameProvider
	{
		public T4PreprocessedClassNameProvider([NotNull] IT4File file) => File = file;

		[NotNull]
		private IT4File File { get; }

		public string GeneratedClassName => File.CreateGeneratedClassName();
		public string GeneratedBaseClassName => GeneratedClassName + "Base";
		public string GeneratedBaseClassFQN => GeneratedBaseClassName;
	}
}
