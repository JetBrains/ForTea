using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveEmptyBlockQuickFix : T4RemoveBlockQuickFixBase<IT4CodeBlock, EmptyBlockHighlighting>
	{
		public override string Text => "Remove empty block";

		public T4RemoveEmptyBlockQuickFix([NotNull] EmptyBlockHighlighting highlighting) : base(highlighting)
		{
		}

		protected override bool ShouldRemove(ITokenNode nextToken)
		{
			if (Node.NodeType == T4ElementTypes.T4ExpressionBlock) return false;
			return base.ShouldRemove(nextToken);
		}
	}
}
