using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.DocumentModel;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format
{
	public sealed class T4CodeBehindFormatProvider : IT4ElementAppendFormatProvider
	{
		public const string _ToStringConversionPrefix = "__To\x200CString(";
		public const string _ToStringConversionSuffix = ")";
		public const string _ExpressionWritingPrefix = "this.Write(";
		public const string _ExpressionWritingSuffix = ");";
		public const string _CodeCommentStart = "/*_T4\x200CCodeStart_*/";
		public const string _CodeCommentEnd = "/*_T4\x200CCodeEnd_*/";

		public string ToStringConversionPrefix => _ToStringConversionPrefix;
		public string ToStringConversionSuffix => _ToStringConversionSuffix;
		public string ExpressionWritingPrefix => _ExpressionWritingPrefix;
		public string ExpressionWritingSuffix => _ExpressionWritingSuffix;
		public string CodeCommentStart => _CodeCommentStart;
		public string CodeCommentEnd => _CodeCommentEnd;
		public string Indent => "";

		public void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset)
		{
		}

		public void AppendMappedOrTrimmed(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.AppendMapped(code);

		private T4CodeBehindFormatProvider()
		{
		}
		
		public static IT4ElementAppendFormatProvider Instance { get; } = new T4CodeBehindFormatProvider();
	}
}
