using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterConfigurableHighlightingsGroup(
	T4HighlightingAttributeGroup.ID,
	T4HighlightingAttributeGroup.ID
)]
[assembly: RegisterHighlighterGroup(
	T4HighlightingAttributeGroup.ID,
	T4HighlightingAttributeGroup.ID,
	HighlighterGroupPriority.LANGUAGE_SETTINGS,
	DemoText =
		"<#@ <T4_DIRECTIVE>assembly</T4_DIRECTIVE> <T4_DIRECTIVE_ATTRIBUTE>name</T4_DIRECTIVE_ATTRIBUTE>=\"<T4_ATTRIBUTE_VALUE>$(</T4_ATTRIBUTE_VALUE><T4_MACRO>SolutionDir</T4_MACRO><T4_ATTRIBUTE_VALUE>)/%</T4_ATTRIBUTE_VALUE><T4_ENVIRONMENT_VARIABLE>USERNAME</T4_ENVIRONMENT_VARIABLE><T4_ATTRIBUTE_VALUE>%/Foo.dll</T4_ATTRIBUTE_VALUE>\" #>"
)]
