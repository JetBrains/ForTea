using System.Collections.Immutable;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Directives
{
	public sealed class IncludeDirectiveInfo : DirectiveInfo
	{
		[NotNull]
		public DirectiveAttributeInfo FileAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo OnceAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public IncludeDirectiveInfo() : base("include")
		{
			FileAttribute = new DirectiveAttributeInfo("file",
				DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);
			OnceAttribute = new BooleanDirectiveAttributeInfo("once", DirectiveAttributeOptions.None);
			SupportedAttributes = ImmutableArray.Create(FileAttribute, OnceAttribute);
		}
	}
}
