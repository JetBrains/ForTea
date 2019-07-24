using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public class T4ExpressionDescription : T4AppendableElementDescriptionBase
	{
		[NotNull]
		private IT4Code Source { get; }

		public T4ExpressionDescription([NotNull] IT4Code source) => Source = source;

		public override void AppendContent(T4CSharpCodeGenerationResult destination, IT4ElementAppendFormatProvider provider)
		{
			destination.Append(provider.Indent);
			destination.AppendLine();
			destination.Append(provider.Indent);
			destination.AppendLine(GetLineDirectiveText(Source));
			destination.Append(provider.Indent);
			destination.Append(provider.ExpressionWritingPrefix);
			destination.Append(provider.ToStringConversionPrefix);
			destination.Append(provider.CodeCommentStart);
			if (IsVisible) provider.AppendMappedOrTrimmed(destination, Source);
			else destination.Append(Source.GetText());
			destination.Append(provider.CodeCommentEnd);
			destination.Append(provider.ToStringConversionSuffix);
			destination.AppendLine(provider.ExpressionWritingSuffix);
			destination.Append(provider.Indent);
			destination.AppendLine();
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}
	}
}
