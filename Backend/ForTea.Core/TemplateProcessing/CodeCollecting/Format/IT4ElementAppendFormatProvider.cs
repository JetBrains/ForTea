using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public interface IT4ElementAppendFormatProvider
	{
		string ToStringConversionPrefix { get; }
		string ToStringConversionSuffix { get; }
		string ExpressionWritingPrefix { get; }
		string ExpressionWritingSuffix { get; }
		string CodeCommentStart { get; }
		string CodeCommentEnd { get; }
		string Indent { get; }

		void AppendMappedOrTrimmed(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4Code code
		);
	}
}
