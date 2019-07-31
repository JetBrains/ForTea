using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveDirectiveQuickFix : T4RemoveBlockQuickFixBase<IT4Directive, IgnoredAssemblyDirectiveHighlighting>
	{
		public override string Text => "Remove directive";

		public T4RemoveDirectiveQuickFix([NotNull] IgnoredAssemblyDirectiveHighlighting highlighting) : base(highlighting)
		{
		}
	}
}
