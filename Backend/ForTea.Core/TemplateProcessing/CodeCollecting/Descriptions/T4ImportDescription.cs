using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4ImportDescription : T4ElementDescriptionBase, IT4AppendableElementDescription
	{
		[NotNull]
		private IT4TreeNode Source { get; }

		[NotNull]
		private string Presentation { get; }

		private T4ImportDescription([NotNull] IT4TreeNode source, [NotNull] string presentation) :
			base(source.GetSourceFile())
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
			IT4ElementAppendFormatProvider provider,
			IPsiSourceFile context
		)
		{
			destination.Append(provider.Indent);
			provider.AppendLineDirective(destination, Source);
			provider.AppendCompilationOffset(destination, Source);
			destination.Append("using ");
			if (HasSameSourceFile(context)) destination.AppendMapped(Source);
			else destination.Append(Presentation);
			destination.AppendLine(";");
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}
	}
}
