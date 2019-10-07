using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4CodeDescription : T4AppendableElementDescriptionBase
	{
		[NotNull]
		private IT4Code Source { get; }

		public T4CodeDescription([NotNull] IT4Code source) : base(source.GetSourceFile()) =>
			Source = source;

		public override void AppendContent(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider,
			IPsiSourceFile context
		)
		{
			destination.Append(provider.Indent);
			destination.AppendLine(GetLineDirectiveText(Source));
			provider.AppendCompilationOffset(destination, GetOffset(Source));
			destination.Append(provider.CodeCommentStart);
			if (HasSameSourceFile(context)) destination.AppendMapped(Source);
			else destination.Append(Source.GetText());
			destination.AppendLine(provider.CodeCommentEnd);
			destination.AppendLine(provider.Indent);
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}
	}
}
