using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4ImportDescription : IT4AppendableElementDescription
	{
		[NotNull]
		private IT4TreeNode Source { get; }

		[NotNull]
		private string Presentation { get; }

		private T4ImportDescription([NotNull] IT4TreeNode source, [NotNull] string presentation)
		{
			Source = source;
			Presentation = presentation;
		}

		[CanBeNull]
		public static T4ImportDescription FromDirective([NotNull] IT4Directive directive)
		{
			(var source, string presentation) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(T4DirectiveInfoManager.Import.NamespaceAttribute.Name);
			if (source == null) return null;
			if (presentation == null) return null;
			return new T4ImportDescription(source, presentation);
		}

		public void AppendContent(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.Indent);
			provider.AppendLineDirective(destination, Source);
			provider.AppendCompilationOffset(destination, Source);
			destination.Append("using ");
			destination.AppendMapped(Source);
			destination.AppendLine(";");
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}
	}
}
