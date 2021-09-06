using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.TextControl.DocumentMarkup;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Attributes
{
[RegisterHighlighter(
	T4HighlightingAttributeIds.BLOCK_TAG,
	GroupId = T4HighlightingAttributeGroup.ID,
	EffectType = EffectType.TEXT,
	ForegroundColor = "#000000",
	BackgroundColor = "#FBFB64",
	Layer = HighlighterLayer.ADDITIONAL_SYNTAX,
	VSPriority = VSPriority.IDENTIFIERS
), RegisterHighlighter(
	 T4HighlightingAttributeIds.CODE_BLOCK,
	 EffectType = EffectType.HIGHLIGHT_ABOVE_TEXT_MARKER,
	 GroupId = T4HighlightingAttributeGroup.ID,
	 Layer = HighlighterLayer.SYNTAX,
	 BackgroundColor = "#F9F9F9",
	 DarkBackgroundColor = "#2C2C2C"
)]
public static class T4ReSharperHighlightingAttributeIds{}
}
