using FluentAssertions;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4ImportDescription : T4ElementDescriptionBase
	{
		[NotNull]
		public ITreeNode Source { get; }

		[NotNull]
		public string Presentation { get; }

		private T4ImportDescription([NotNull] ITreeNode source, [NotNull] string presentation) :
			base (source.GetContainingFile().As<IT4File>().NotNull())
		{
			Source = source;
			Presentation = presentation;
		}

		[CanBeNull]
		public static T4ImportDescription FromDirective(
			[NotNull] IT4Directive directive
		)
		{
			(var source, string presentation) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(T4DirectiveInfoManager.Import.NamespaceAttribute.Name);
			if (source == null) return null;
			if (presentation == null) return null;
			return new T4ImportDescription(source, presentation);
		}
	}
}
