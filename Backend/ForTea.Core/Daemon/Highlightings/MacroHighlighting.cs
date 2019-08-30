using GammaJul.ForTea.Core.Daemon.Attributes.GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Daemon.Tooltip;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[DaemonTooltipProvider(typeof(IT4MacroTooltipProvider))]
	[StaticSeverityHighlighting(
		Severity.INFO,
		HighlightingGroupIds.IdentifierHighlightingsGroup,
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public class MacroHighlighting : ICustomAttributeIdHighlighting
	{
		[NotNull]
		public string AttributeId => T4HighlightingAttributeIds.MACRO;

		[NotNull]
		public string ToolTip => string.Empty;

		[NotNull]
		public string ErrorStripeToolTip => string.Empty;

		private DocumentRange Range { get; }
		public MacroHighlighting(DocumentRange range) => Range = range;
		public bool IsValid() => true;
		public DocumentRange CalculateRange() => Range;
	}
}
