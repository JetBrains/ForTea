using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	[QuickFix]
	public class T4RemoveRedundantIncludeQuickFix :
		T4RemoveBlockQuickFixBase<IT4Directive, T4RedundantIncludeHighlighting>
	{
		public override string Text => "Remove redundant include";

		public T4RemoveRedundantIncludeQuickFix([NotNull] T4RedundantIncludeHighlighting highlighting) :
			base(highlighting)
		{
		}
	}
}
