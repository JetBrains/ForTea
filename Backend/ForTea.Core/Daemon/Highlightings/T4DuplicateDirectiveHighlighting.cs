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
		AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE
	)]
	public class T4DuplicateDirectiveHighlighting : T4HighlightingBase<IT4Directive>
	{
		public T4DuplicateDirectiveHighlighting([NotNull] IT4Directive associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Duplicate directive";
	}
}
