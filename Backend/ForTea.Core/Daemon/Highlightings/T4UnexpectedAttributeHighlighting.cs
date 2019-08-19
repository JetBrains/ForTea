using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.WARNING,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.WARNING,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.WARNING_ATTRIBUTE
	)]
	public sealed class T4UnexpectedAttributeHighlighting : T4HighlightingBase<ITokenNode>
	{
		public T4UnexpectedAttributeHighlighting([NotNull] ITokenNode associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Unexpected attribute";
	}
}
