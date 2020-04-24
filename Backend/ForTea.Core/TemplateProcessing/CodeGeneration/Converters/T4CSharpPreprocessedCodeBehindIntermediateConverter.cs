using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpPreprocessedCodeBehindIntermediateConverter :
		T4CSharpCodeBehindIntermediateConverterBase
	{
		public T4CSharpPreprocessedCodeBehindIntermediateConverter([NotNull] IT4File file) : base(file)
		{
		}

		protected override string GeneratedClassName => File.CreateGeneratedClassName();
		protected override string GeneratedBaseClassName => GeneratedClassName + "Base";
		protected override string GeneratedBaseClassFQN => GeneratedBaseClassName;
	}
}
