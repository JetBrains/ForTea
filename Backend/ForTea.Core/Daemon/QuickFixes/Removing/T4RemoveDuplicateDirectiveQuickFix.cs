using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveDuplicateDirectiveQuickFix :
		T4RemoveBlockQuickFixBase<IT4Directive, DuplicateDirectiveWarning>
	{
		public override string Text => "Remove duplicate directive";
		protected override IT4Directive Node => Highlighting.Directive;

		public T4RemoveDuplicateDirectiveQuickFix([NotNull] DuplicateDirectiveWarning highlighting) : base(highlighting)
		{
		}
	}
}
