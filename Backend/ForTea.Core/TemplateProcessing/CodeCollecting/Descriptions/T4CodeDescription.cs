using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4CodeDescription : IT4AppendableElementDescription
	{
		[NotNull]
		private IT4Code Source { get; }

		public T4CodeDescription([NotNull] IT4Code source) => Source = source;

		public void AppendContent(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.Indent);
			provider.AppendLineDirective(destination, Source);
			provider.AppendCompilationOffset(destination, Source);
			destination.Append(provider.CodeCommentStart);
			destination.AppendMapped(Source);
			destination.AppendLine(provider.CodeCommentEnd);
			destination.AppendLine(provider.Indent);
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}
	}
}
