using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes.Idea;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.MACRO,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = HighlightingAttributeIds.KEYWORD
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = HighlightingAttributeIds.NUMBER
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.ATTRIBUTE_VALUE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_VALUE
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.DIRECTIVE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_TAG
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_NAME
)]
