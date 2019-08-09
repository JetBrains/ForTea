using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveExpressionBlockQuickFix :
		T4RemoveBlockQuickFixBase<T4ExpressionBlock, T4EmptyExpressionBlockHighlighting>
	{
		public override string Text => "Remove empty expression block";

		public T4RemoveExpressionBlockQuickFix([NotNull] T4EmptyExpressionBlockHighlighting highlighting) :
			base(highlighting)
		{
		}

		protected override bool ShouldRemove(ITokenNode nextToken) => false;
	}
}
