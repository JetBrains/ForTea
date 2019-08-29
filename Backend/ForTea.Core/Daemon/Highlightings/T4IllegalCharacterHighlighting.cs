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
		OverlapResolve = OverlapResolveKind.UNRESOLVED_ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.UNRESOLVED_ERROR_ATTRIBUTE
	)]
	public class T4IllegalCharacterHighlighting : T4HighlightingBase<IT4AttributeValue>
	{
		private DocumentRange Range { get; }
		public override string ToolTip => "Illegal character";

		public T4IllegalCharacterHighlighting(
			[NotNull] IT4AttributeValue associatedNode,
			DocumentRange range) : base(associatedNode
		) => Range = range;

		public override DocumentRange CalculateRange() => Range;
	}
}
