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
	public sealed class MacroHighlighting : ICustomAttributeIdHighlighting
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

	[DaemonTooltipProvider(typeof(IT4EnvironmentVariableTooltipProvider))]
	[StaticSeverityHighlighting(
		Severity.INFO,
		HighlightingGroupIds.IdentifierHighlightingsGroup,
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public sealed class EnvironmentVariableHighlighting : ICustomAttributeIdHighlighting
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

	[StaticSeverityHighlighting(
		Severity.INFO,
		HighlightingGroupIds.IdentifierHighlightingsGroup,
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public sealed class DirectiveHighlighting : ICustomAttributeIdHighlighting
	{
		[NotNull]
		public string AttributeId => T4HighlightingAttributeIds.DIRECTIVE;

		public bool IsValid() => true;
		private DocumentRange Range { get; }
		public DirectiveHighlighting(DocumentRange range) => Range = range;
		public DocumentRange CalculateRange() => Range;

		[NotNull]
		public string ToolTip => string.Empty;

		[NotNull]
		public string ErrorStripeToolTip => string.Empty;
	}

	[StaticSeverityHighlighting(
		Severity.INFO,
		HighlightingGroupIds.IdentifierHighlightingsGroup,
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public sealed class DirectiveAttributeHighlighting : ICustomAttributeIdHighlighting
	{
		[NotNull]
		public string AttributeId => T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE;

		public bool IsValid() => true;
		private DocumentRange Range { get; }
		public DirectiveAttributeHighlighting(DocumentRange range) => Range = range;
		public DocumentRange CalculateRange() => Range;

		[NotNull]
		public string ToolTip => string.Empty;

		[NotNull]
		public string ErrorStripeToolTip => string.Empty;
	}
}
