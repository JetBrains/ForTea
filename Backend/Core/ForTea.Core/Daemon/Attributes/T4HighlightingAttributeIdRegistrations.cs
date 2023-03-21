using GammaJul.ForTea.Core.Resources;
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
     RiderPresentableNameResourceName = nameof(Strings.T4HighlighterMacro_Text)
   ),
   RegisterHighlighter(
     ENVIRONMENT_VARIABLE,
     EffectType = EffectType.TEXT,
     GroupId = T4HighlightingAttributeGroup.ID,
     FallbackAttributeId = DefaultLanguageAttributeIds.NUMBER,
     RiderPresentableNameResourceName = nameof(Strings.T4HighlighterEnvironmentVariable_Text)
   ), RegisterHighlighter(
     ATTRIBUTE_VALUE,
     EffectType = EffectType.TEXT,
     GroupId = T4HighlightingAttributeGroup.ID,
     FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_VALUE,
     RiderPresentableNameResourceName = nameof(Strings.T4HighlighterAttributeValue_Text)
   ), RegisterHighlighter(
     DIRECTIVE,
     EffectType = EffectType.TEXT,
     GroupId = T4HighlightingAttributeGroup.ID,
     FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_TAG,
     RiderPresentableNameResourceName = nameof(Strings.T4HihglighterDirectiveName_Text)
   ), RegisterHighlighter(
     DIRECTIVE_ATTRIBUTE,
     EffectType = EffectType.TEXT,
     GroupId = T4HighlightingAttributeGroup.ID,
     FallbackAttributeId = IdeaXmlHighlighterColorsAttributeIds.XML_ATTRIBUTE_NAME,
     RiderPresentableNameResourceName = nameof(Strings.T4highlighterAttributeName_Text)
   )]
  public static partial class T4HighlightingAttributeIds
  {
  }
}