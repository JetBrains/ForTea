using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4ImportDescription : T4ElementDescriptionBase
	{
		[NotNull]
		public ITreeNode Source { get; }

		[NotNull]
		public string Presentation { get; }

		private T4ImportDescription([NotNull] ITreeNode source, [NotNull] string presentation)
		{
			Source = source;
			Presentation = presentation;
		}

		[CanBeNull]
		public static T4ImportDescription FromDirective(
			[NotNull] IT4Directive directive,
			[NotNull] T4DirectiveInfoManager manager
		)
		{
			(var source, string presentation) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(manager.Import.NamespaceAttribute.Name);
			if (source == null) return null;
			if (presentation == null) return null;
			return new T4ImportDescription(source, presentation);
		}
	}
}
