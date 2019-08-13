using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
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
	public sealed class T4EmptyExpressionBlockHighlighting : T4HighlightingBase<T4ExpressionBlock>
	{
		public T4EmptyExpressionBlockHighlighting([NotNull] T4ExpressionBlock associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Expression block cannot be empty";
	}
}
