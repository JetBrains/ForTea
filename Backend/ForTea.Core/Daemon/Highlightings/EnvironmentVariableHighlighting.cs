using GammaJul.ForTea.Core.Daemon.Attributes.GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Daemon.Tooltip;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[DaemonTooltipProvider(typeof(IT4EnvironmentVariableTooltipProvider))]
	[StaticSeverityHighlighting(
		Severity.INFO,
		HighlightingGroupIds.IdentifierHighlightingsGroup,
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public class EnvironmentVariableHighlighting : ICustomAttributeIdHighlighting
	{
		[NotNull]
		public string AttributeId => T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE;

		public bool IsValid() => true;
		private DocumentRange Range { get; }
		public EnvironmentVariableHighlighting(DocumentRange range) => Range = range;
		public DocumentRange CalculateRange() => Range;

		[NotNull]
		public string ToolTip => string.Empty;

		[NotNull]
		public string ErrorStripeToolTip => string.Empty;
	}
}
