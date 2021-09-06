using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.Rider.Backend.Features.SyntaxHighlighting.Web;
using JetBrains.TextControl.DocumentMarkup;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Attributes
{
	[RegisterHighlighter(
		T4HighlightingAttributeIds.CODE_BLOCK,
		EffectType = EffectType.HIGHLIGHT_ABOVE_TEXT_MARKER,
		GroupId = T4HighlightingAttributeGroup.ID,
		FallbackAttributeId = RazorSyntaxHighlightingAttributeIds.RAZOR_CODE_BLOCK,
		Layer = HighlighterLayer.SYNTAX,
		BackgroundColor = "#FBFBFB",
		DarkBackgroundColor = "#303030",
		RiderPresentableName = "Code Block"
	)]
	public static class T4RiderHighlightingAttributeIds
	{
	}
}