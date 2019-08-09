using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.DocumentModel;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public sealed class T4PreprocessCodeFormatProvider : T4RealCodeFormatProvider
	{
		public T4PreprocessCodeFormatProvider(string indent) : base(indent)
		{
		}

		public override void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset)
		{
			// In preprocessed file, behave like VS
		}

		public override bool ShouldBreakExpressionWithLineDirective => false;

		public override void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.Append(code.GetText().Trim());
	}
}
