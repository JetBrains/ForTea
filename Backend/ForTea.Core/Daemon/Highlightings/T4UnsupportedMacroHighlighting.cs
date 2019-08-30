using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class T4UnsupportedMacroHighlighting : T4HighlightingBase<IT4Macro>
	{
		public T4UnsupportedMacroHighlighting([NotNull] IT4Macro associatedNode) : base(associatedNode)
		{
		}

		public override DocumentRange CalculateRange() =>
			AssociatedNode.RawAttributeValue?.GetHighlightingRange() ?? DocumentRange.InvalidRange;

		public override string ToolTip => "This macro is not supported yet";
	}
}
