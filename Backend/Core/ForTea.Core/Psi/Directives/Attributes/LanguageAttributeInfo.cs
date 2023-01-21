using System.Collections.Immutable;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Directives.Attributes
{
  public sealed class LanguageAttributeInfo : DirectiveAttributeInfo
  {
    [NotNull] public const string CSharpLanguageAttributeValue = "C#";
    [NotNull] public const string NewCSharpLanguageAttributeValue = "C#v3.5";
    [NotNull] public const string VBLanguageAttributeValue = "VB";

    [NotNull, ItemNotNull]
    public static ImmutableArray<string> PromotedValues { get; } = ImmutableArray.Create(
      CSharpLanguageAttributeValue,
      NewCSharpLanguageAttributeValue
    );

    [NotNull, ItemNotNull]
    private ImmutableArray<string> OtherValues { get; } = new[] { VBLanguageAttributeValue }.ToImmutableArray();

    public override bool IsValid(string value) => PromotedValues.Contains(value) || OtherValues.Contains(value);
    public override ImmutableArray<string> IntelliSenseValues => PromotedValues;

    public LanguageAttributeInfo() : base("language", DirectiveAttributeOptions.None)
    {
    }
  }
}