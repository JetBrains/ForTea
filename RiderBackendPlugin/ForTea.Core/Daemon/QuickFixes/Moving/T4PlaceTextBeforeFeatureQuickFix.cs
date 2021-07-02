using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Moving
{
	[QuickFix]
	public sealed class T4PlaceTextBeforeFeatureQuickFix :
		PlaceBeforeFeatureQuickFixBase<IT4Token, TextAfterFeatureError>
	{
		public T4PlaceTextBeforeFeatureQuickFix([NotNull] TextAfterFeatureError highlighting) : base(highlighting)
		{
		}

		protected override IT4Token Node => Highlighting.Text;
	}
}
