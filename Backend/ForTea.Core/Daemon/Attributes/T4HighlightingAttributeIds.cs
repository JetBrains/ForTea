using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighterGroup(
	"T4",
	T4HighlightingAttributeGroup.ID,
	HighlighterGroupPriority.LANGUAGE_SETTINGS,
	DemoText =
		"<#@ <T4_DIRECTIVE>assembly</T4_DIRECTIVE> <T4_DIRECTIVE_ATTRIBUTE>name</T4_DIRECTIVE_ATTRIBUTE>=\"<T4_ATTRIBUTE_VALUE>$(</T4_ATTRIBUTE_VALUE><T4_MACRO_VALUE>SolutionDir</T4_MACRO_VALUE><T4_ATTRIBUTE_VALUE>)/%</T4_ATTRIBUTE_VALUE><T4_ENVIRONMENT_VARIABLE>USERNAME</T4_ENVIRONMENT_VARIABLE><T4_ATTRIBUTE_VALUE>%<T4_ATTRIBUTE_VALUE>/Foo.dll</T4_ATTRIBUTE_VALUE>\"#>"
)]
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

namespace GammaJul.ForTea.Core.Daemon.Attributes
{
	public static class T4HighlightingAttributeIds
	{
		public const string MACRO = "T4 Macro";
		public const string ENVIRONMENT_VARIABLE = "T4 Environment Variable";
	}
}
