using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings
{
	[RegisterHighlighter(Identifier, EffectType = EffectType.TEXT),
	 RegisterHighlighter(UserType, EffectType = EffectType.TEXT),
	 RegisterHighlighter(UserTypeDelegate, EffectType = EffectType.TEXT),
	 RegisterHighlighter(UserTypeEnum, EffectType = EffectType.TEXT),
	 RegisterHighlighter(UserTypeInterface, EffectType = EffectType.TEXT),
	 RegisterHighlighter(UserTypeTypeParameter, EffectType = EffectType.TEXT),
	 RegisterHighlighter(UserTypeValueType, EffectType = EffectType.TEXT)]
	public static class PredefinedHighlighterIds
	{
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
	}
}
