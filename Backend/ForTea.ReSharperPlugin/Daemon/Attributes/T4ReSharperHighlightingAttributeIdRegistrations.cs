using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Attributes;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.BLOCK_TAG,
	GroupId = T4HighlightingAttributeGroup.ID,
	EffectType = EffectType.TEXT,
	ForegroundColor = "#000000",
	BackgroundColor = "#FBFB64",
	Layer = HighlighterLayer.ADDITIONAL_SYNTAX,
	VSPriority = VSPriority.IDENTIFIERS
)]
