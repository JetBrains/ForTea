using JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(PredefinedHighlighterIds.Identifier, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.UserType, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.UserTypeDelegate, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.UserTypeEnum, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.UserTypeInterface, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.UserTypeTypeParameter, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.UserTypeValueType, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.HtmlServerSideScript, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.HtmlElementName, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.HtmlAttributeName, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(PredefinedHighlighterIds.HtmlAttributeValue, EffectType = EffectType.TEXT)]

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings {
	public static class PredefinedHighlighterIds {
		public const string Comment = HighlightingAttributeIds.COMMENT;
		public const string Identifier = "Identifier";
		public const string Keyword = HighlightingAttributeIds.KEYWORD;
		public const string Number = HighlightingAttributeIds.NUMBER;
		public const string Operator = HighlightingAttributeIds.OPERATOR_IDENTIFIER_ATTRIBUTE;
		public const string String = HighlightingAttributeIds.STRING;
		public const string UserType = "User Types";
		public const string UserTypeDelegate = "User Types(Delegates)";
		public const string UserTypeEnum = "User Types(Enums)";
		public const string UserTypeInterface = "User Types(Interfaces)";
		public const string UserTypeTypeParameter = "User Types(Type parameters)";
		public const string UserTypeValueType = "User Types(Value types)";
		// Black magic by GammaJul.
		// Cannot get the code working without it
		public const string HtmlServerSideScript = "HTML Server-Side Script";
		public const string HtmlElementName = "HTML Element Name";
		public const string HtmlAttributeName = "HTML Attribute Name";
		public const string HtmlAttributeValue = "HTML Attribute Value";
	}

}
