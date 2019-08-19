using GammaJul.ForTea.Core.Daemon.Attributes.GammaJul.ForTea.Core.Daemon.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighterGroup(
	T4HighlightingAttributeIds.GROUP_ID,
	T4HighlightingAttributeIds.GROUP_ID,
	HighlighterGroupPriority.LANGUAGE_SETTINGS
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.MACRO,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeIds.GROUP_ID,
	FallbackAttributeId = HighlightingAttributeIds.KEYWORD
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeIds.GROUP_ID,
	FallbackAttributeId = HighlightingAttributeIds.DOC_COMMENT
)]
[assembly: RegisterHighlighter(
	T4HighlightingAttributeIds.RAW_ATTRIBUTE_VALUE,
	EffectType = EffectType.TEXT,
	GroupId = T4HighlightingAttributeIds.GROUP_ID,
	FallbackAttributeId = HighlightingAttributeIds.STRING
)]