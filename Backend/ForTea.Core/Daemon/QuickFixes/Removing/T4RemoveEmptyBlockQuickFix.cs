using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveEmptyBlockQuickFix : T4RemoveBlockQuickFixBase<IT4CodeBlock, EmptyBlockHighlighting>
	{
		public override string Text => "Remove empty block";

		public T4RemoveEmptyBlockQuickFix([NotNull] EmptyBlockHighlighting highlighting) : base(highlighting)
		{
		}
	}
}
