using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.DocumentModel;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public class T4RealCodeFormatProvider : IT4ElementAppendFormatProvider
	{
		public string ToStringConversionPrefix => "this.ToStringHelper.ToStringWithCulture(";
		public string ToStringConversionSuffix => ")";
		public string ExpressionWritingPrefix => "this.Write(";
		public string ExpressionWritingSuffix => ");";
		public string CodeCommentStart => "";
		public string CodeCommentEnd => "";
		public string Indent { get; }
		public virtual bool ShouldBreakExpressionWithLineDirective => true;

		public virtual void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset)
		{
			for (var i = Int32<DocColumn>.O; i < offset; i++)
			{
				destination.Append(" ");
			}
		}

		// Ranges map from this generator are ignored anyway
		public virtual void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.Append(code.GetText());

		public T4RealCodeFormatProvider(string indent) => Indent = indent;
	}
}
