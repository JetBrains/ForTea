using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.UNRESOLVED_ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.UNRESOLVED_ERROR_ATTRIBUTE
	)]
	public class T4UnresolvedMacroHighlighting : T4HighlightingBase<IT4Macro>
	{
		public T4UnresolvedMacroHighlighting([NotNull] IT4Macro associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Unresolved macro";
	}
}
