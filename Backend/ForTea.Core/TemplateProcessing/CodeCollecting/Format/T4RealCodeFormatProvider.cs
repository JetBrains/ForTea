using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public sealed class T4RealCodeFormatProvider : IT4ElementAppendFormatProvider
	{
		public string ToStringConversionPrefix => "this.ToStringHelper.ToStringWithCulture(";
		public string ToStringConversionSuffix => ")";
		public string ExpressionWritingPrefix => "this.Write(";
		public string ExpressionWritingSuffix => ");";
		public string CodeCommentStart => "";
		public string CodeCommentEnd => "";
		public string Indent { get; }

		// Ranges map from this generator are ignored anyway
		public void AppendMappedOrTrimmed(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.Append(code.GetText().Trim());

		public T4RealCodeFormatProvider(string indent) => Indent = indent;
	}
}
