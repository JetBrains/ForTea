using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpPreprocessedCodeBehindIntermediateConverter :
		T4CSharpCodeBehindIntermediateConverterBase
	{
		public T4CSharpPreprocessedCodeBehindIntermediateConverter([NotNull] IT4File file) : base(
			file,
			new T4PreprocessedClassNameProvider(file)
		)
		{
		}
	}
}
