using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.DocumentModel;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public sealed class T4CodeBehindFormatProvider : IT4ElementAppendFormatProvider
	{
		public string ToStringConversionPrefix => "__To\x200CString(";
		public string ToStringConversionSuffix => ")";
		public string ExpressionWritingPrefix => "this.Write(";
		public string ExpressionWritingSuffix => ");";
		public string CodeCommentStart => "/*_T4\x200CCodeStart_*/";
		public string CodeCommentEnd => "/*_T4\x200CCodeEnd_*/";
		public string Indent => "";
		public bool ShouldBreakExpressionWithLineDirective => false;

		public void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset)
		{
		}

		public void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.AppendMapped(code);

		private T4CodeBehindFormatProvider()
		{
		}
		
		public static IT4ElementAppendFormatProvider Instance { get; } = new T4CodeBehindFormatProvider();
	}
}
