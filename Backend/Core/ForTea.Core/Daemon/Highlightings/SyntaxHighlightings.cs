using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.INFO,
		typeof(HighlightingGroupIds.IdentifierHighlightings),
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

	[StaticSeverityHighlighting(
		Severity.INFO,
		typeof(HighlightingGroupIds.IdentifierHighlightings),
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
		typeof(ReSharperSyntaxHighlightings),
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public sealed class T4CodeBlockHighlighting : ReSharperSyntaxHighlighting, ICustomFrontendSeverityHighlighting
	{
		public T4CodeBlockHighlighting(DocumentRange range) : base(T4HighlightingAttributeIds.CODE_BLOCK, null, range)
		{
		}

		public string FrontendSeverity => FrontendHighlighterSeverities.BACKGROUND;
	}
}
