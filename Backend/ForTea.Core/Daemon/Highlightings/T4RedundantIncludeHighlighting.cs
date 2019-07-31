using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.HINT,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.HINT_ATTRIBUTE
	)]
	public class T4RedundantIncludeHighlighting : T4HighlightingBase<IT4Directive>
	{
		public T4RedundantIncludeHighlighting([NotNull] IT4Directive associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "File already included";
	}
}
