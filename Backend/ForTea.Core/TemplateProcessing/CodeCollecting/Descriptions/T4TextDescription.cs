using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public class T4TextDescription : T4AppendableElementDescriptionBase
	{
		[NotNull]
		private string Text { get; }

		public T4TextDescription([NotNull] string text) => Text = text;

		public override void AppendContent(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider,
			IT4File context
		)
		{
			destination.Append(provider.Indent);
			destination.Append(provider.ExpressionWritingPrefix);
			destination.Append("\"");
			destination.Append(Text);
			destination.Append("\"");
			destination.AppendLine(provider.ExpressionWritingSuffix);
		}
	}
}
