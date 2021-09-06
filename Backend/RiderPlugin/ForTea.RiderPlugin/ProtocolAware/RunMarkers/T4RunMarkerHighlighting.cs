using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Rider.Backend.Features.RunMarkers;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.RunMarkers
{
	[StaticSeverityHighlighting(
		Severity.INFO,
		typeof(RunMarkerHighlighting.RunMarkers),
		OverlapResolve = OverlapResolveKind.NONE
	)]
	public sealed class T4RunMarkerHighlighting : ICustomAttributeIdHighlighting
	{
		[NotNull] private const string Tooltip = "Run T4 File";

		[NotNull]
		public IT4TemplateDirective Directive { get; }

		public T4RunMarkerHighlighting([NotNull] IT4TemplateDirective directive) => Directive = directive;
		public bool IsValid() => Directive.IsValid() && CalculateRange().IsValid();
		public DocumentRange CalculateRange() => Directive.GetHighlightingRange();

		[NotNull]
		public string ToolTip => Tooltip;

		[NotNull]
		public string ErrorStripeToolTip => Tooltip;

		[NotNull]
		public string AttributeId => T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID;
	}
}
