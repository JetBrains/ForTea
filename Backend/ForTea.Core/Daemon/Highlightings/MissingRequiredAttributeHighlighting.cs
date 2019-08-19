using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class MissingRequiredAttributeHighlighting : T4HighlightingBase<ITokenNode>
	{
		[NotNull]
		private string MissingAttributeName { get; }

		[NotNull]
		public override string ToolTip => "Missing required attribute \"" + MissingAttributeName + "\"";

		public MissingRequiredAttributeHighlighting(
			[NotNull] ITokenNode directiveNameNode,
			[NotNull] string missingAttributeName
		) : base(directiveNameNode) => MissingAttributeName = missingAttributeName;
	}
}
