using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveDuplicateDirectiveQuickFix :
		T4RemoveBlockQuickFixBase<IT4Directive, T4DuplicateDirectiveHighlighting>
	{
		public override string Text => "Remove duplicate directive";

		public T4RemoveDuplicateDirectiveQuickFix([NotNull] T4DuplicateDirectiveHighlighting highlighting) :
			base(highlighting)
		{
		}
	}
}
