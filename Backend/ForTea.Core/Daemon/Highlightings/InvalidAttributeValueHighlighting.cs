using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class InvalidAttributeValueHighlighting : T4HighlightingBase<IT4Token>
	{
		[CanBeNull]
		public DirectiveAttributeInfo DirectiveAttributeInfo { get; }

		[NotNull]
		public override string ToolTip { get; }

		public InvalidAttributeValueHighlighting(
			[NotNull] IT4Token associatedNode,
			[CanBeNull] DirectiveAttributeInfo directiveAttributeInfo,
			[NotNull] string errorMessage
		)
			: base(associatedNode)
		{
			DirectiveAttributeInfo = directiveAttributeInfo;
			ToolTip = errorMessage;
		}
	}
}
