using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes.Idea;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.MACRO,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = DefaultLanguageAttributeIds.KEYWORD,
	RiderPresentableName = "Macro"
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = DefaultLanguageAttributeIds.NUMBER,
	RiderPresentableName = "Environment Variable"
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.ATTRIBUTE_VALUE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_VALUE,
	RiderPresentableName = "Attribute Value"
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.DIRECTIVE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_TAG,
	RiderPresentableName = "Directive Name"
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_NAME,
	RiderPresentableName = "Attribute Name"
)]
