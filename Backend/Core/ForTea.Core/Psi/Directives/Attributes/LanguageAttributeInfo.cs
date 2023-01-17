using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ForTea.Core.Psi.Directives.Attributes
{
	public sealed class LanguageAttributeInfo : DirectiveAttributeInfo
	{
		[NotNull] public const string CSharpLanguageAttributeValue = "C#";
		[NotNull] public const string NewCSharpLanguageAttributeValue = "C#v3.5";
		[NotNull] public const string VBLanguageAttributeValue = "VB";

		[NotNull, ItemNotNull]
		public static JetImmutableArray<string> PromotedValues { get; } = new[]
		{
			CSharpLanguageAttributeValue,
			NewCSharpLanguageAttributeValue
		}.ToImmutableArray();

		[NotNull, ItemNotNull]
		private JetImmutableArray<string> OtherValues { get; } = new[] {VBLanguageAttributeValue}.ToImmutableArray();

		public override bool IsValid(string value) => PromotedValues.Contains(value) || OtherValues.Contains(value);
		public override JetImmutableArray<string> IntelliSenseValues => PromotedValues;

		public LanguageAttributeInfo() : base("language", DirectiveAttributeOptions.None)
		{
		}
	}
}
