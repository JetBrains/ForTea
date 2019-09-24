using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ForTea.RiderPlugin.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon.IdeaAttributes;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(
	T4RiderHighlightAttributeIds.RAW_ATTRIBUTE_VALUE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_VALUE
)]
[assembly: RegisterHighlighter(
	T4RiderHighlightAttributeIds.DIRECTIVE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_TAG
)]
[assembly: RegisterHighlighter(
	T4RiderHighlightAttributeIds.DIRECTIVE_ATTRIBUTE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeGroup.ID,
	FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_NAME
)]

namespace JetBrains.ForTea.RiderPlugin.Daemon.Attributes
{
	public static class T4RiderHighlightAttributeIds
	{
		public const string RAW_ATTRIBUTE_VALUE = "T4 Raw Attribute Value";
		public const string DIRECTIVE = "T4 Directive";
		public const string DIRECTIVE_ATTRIBUTE = "T4 Directive Attribute";
	}
}