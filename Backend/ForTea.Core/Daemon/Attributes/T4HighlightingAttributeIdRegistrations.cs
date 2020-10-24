using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes.Idea;
using JetBrains.TextControl.DocumentMarkup;

namespace GammaJul.ForTea.Core.Daemon.Attributes
{
[RegisterHighlighter(
	MACRO,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = DefaultLanguageAttributeIds.KEYWORD,
	RiderPresentableName = "Macro"
),
RegisterHighlighter(
	ENVIRONMENT_VARIABLE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = DefaultLanguageAttributeIds.NUMBER,
	RiderPresentableName = "Environment Variable"
),RegisterHighlighter(
	ATTRIBUTE_VALUE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_VALUE,
	RiderPresentableName = "Attribute Value"
),RegisterHighlighter(
	DIRECTIVE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_TAG,
	RiderPresentableName = "Directive Name"
),RegisterHighlighter(
	DIRECTIVE_ATTRIBUTE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_NAME,
	RiderPresentableName = "Attribute Name"
)]
public static partial class T4HighlightingAttributeIds{}
}