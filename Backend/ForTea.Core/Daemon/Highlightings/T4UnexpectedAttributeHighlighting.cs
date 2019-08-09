using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.WARNING,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.WARNING_ATTRIBUTE
	)]
	public class T4UnexpectedAttributeHighlighting : T4HighlightingBase<IT4Token>
	{
		public T4UnexpectedAttributeHighlighting([NotNull] IT4Token associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Unexpected attribute";
	}
}
