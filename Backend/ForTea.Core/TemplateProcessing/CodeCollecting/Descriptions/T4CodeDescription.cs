using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public class T4CodeDescription : T4AppendableElementDescriptionBase
	{
		[NotNull]
		private IT4Code Source { get; }

		public T4CodeDescription([NotNull] IT4Code source) => Source = source;

		public override void AppendContent(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider
		)
		{
			provider.AppendCompilationOffset(destination, GetOffset(Source));
			destination.Append(provider.Indent);
			destination.AppendLine(GetLineDirectiveText(Source));
			destination.Append(provider.CodeCommentStart);
			if (IsVisible) destination.AppendMapped(Source);
			else destination.Append(Source.GetText());
			destination.AppendLine(provider.CodeCommentEnd);
			destination.AppendLine(provider.Indent);
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}

		private static Int32<DocColumn> GetOffset(ITreeNode node) =>
			node.GetSourceFile().NotNull().Document.GetCoordsByOffset(node.GetTreeStartOffset().Offset).Column;
	}
}
