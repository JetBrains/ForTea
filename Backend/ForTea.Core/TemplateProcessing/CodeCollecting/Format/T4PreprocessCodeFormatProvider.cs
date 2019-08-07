using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public sealed class T4PreprocessCodeFormatProvider : T4RealCodeFormatProvider
	{
		public T4PreprocessCodeFormatProvider(string indent) : base(indent)
		{
		}

		public override bool ShouldBreakExpressionWithLineDirective => false;

		public override void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.Append(code.GetText().Trim());
	}
}
