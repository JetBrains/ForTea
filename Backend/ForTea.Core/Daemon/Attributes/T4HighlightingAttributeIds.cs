using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;

namespace GammaJul.ForTea.Core.Daemon.Attributes
{
	[RegisterConfigurableHighlightingsGroup(
		 T4HighlightingAttributeGroup.ID,
		 T4HighlightingAttributeGroup.ID
	 ), RegisterHighlighterGroup(
		 T4HighlightingAttributeGroup.ID,
		 T4HighlightingAttributeGroup.ID,
		 HighlighterGroupPriority.LANGUAGE_SETTINGS,
		 DemoText =
			 "<#@ <T4_DIRECTIVE>assembly</T4_DIRECTIVE> <T4_DIRECTIVE_ATTRIBUTE>name</T4_DIRECTIVE_ATTRIBUTE>=\"<T4_ATTRIBUTE_VALUE>$(</T4_ATTRIBUTE_VALUE><T4_MACRO>SolutionDir</T4_MACRO><T4_ATTRIBUTE_VALUE>)/%</T4_ATTRIBUTE_VALUE><T4_ENVIRONMENT_VARIABLE>USERNAME</T4_ENVIRONMENT_VARIABLE><T4_ATTRIBUTE_VALUE>%/Foo.dll</T4_ATTRIBUTE_VALUE>\" #>"
	 )]
	public static partial class T4HighlightingAttributeIds
	{
		public const string MACRO = "T4 Macro";
		public const string ENVIRONMENT_VARIABLE = "T4 Environment Variable";
		public const string BLOCK_TAG = "T4 Block Tag";
		public const string DIRECTIVE = "T4 Directive";
		public const string DIRECTIVE_ATTRIBUTE = "T4 Directive Attribute";
		public const string ATTRIBUTE_VALUE = "T4 Attribute Value";
	}
}
