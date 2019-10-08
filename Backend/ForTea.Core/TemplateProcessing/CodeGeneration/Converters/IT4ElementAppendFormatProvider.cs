using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public interface IT4ElementAppendFormatProvider
	{
		string ToStringConversionPrefix { get; }
		string ToStringConversionSuffix { get; }
		string ExpressionWritingPrefix { get; }
		string ExpressionWritingSuffix { get; }
		string CodeCommentStart { get; }
		string CodeCommentEnd { get; }
		string ExpressionCommentStart { get; }
		string ExpressionCommentEnd { get; }
		string Indent { get; }
		bool ShouldBreakExpressionWithLineDirective { get; }

		void AppendCompilationOffset(
			[NotNull] T4CSharpCodeGenerationResult destination,
			Int32<DocColumn> offset
		);

		void AppendMappedIfNeeded(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4Code code
		);
	}
}
