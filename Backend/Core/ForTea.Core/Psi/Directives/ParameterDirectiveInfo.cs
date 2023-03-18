using System.Collections.Immutable;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Directives
{
  public class ParameterDirectiveInfo : DirectiveInfo
  {
    [NotNull] public DirectiveAttributeInfo TypeAttribute { get; }

    [NotNull] public DirectiveAttributeInfo NameAttribute { get; }

    public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

    public ParameterDirectiveInfo()
      : base("parameter")
    {
      TypeAttribute = new DirectiveAttributeInfo("type", DirectiveAttributeOptions.Required);
      NameAttribute = new DirectiveAttributeInfo("name",
        DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);
      SupportedAttributes = ImmutableArray.Create(TypeAttribute, NameAttribute);
    }
  }
}