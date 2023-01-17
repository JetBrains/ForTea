using System.Collections.Immutable;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Directives {

	public class OutputDirectiveInfo : DirectiveInfo {

		[NotNull]
		public DirectiveAttributeInfo ExtensionAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo EncodingAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public OutputDirectiveInfo()
			: base("output") {
			ExtensionAttribute = new DirectiveAttributeInfo("extension", DirectiveAttributeOptions.None);
			EncodingAttribute = new EncodingDirectiveAttributeInfo(DirectiveAttributeOptions.None);
			SupportedAttributes = ImmutableArray.Create(ExtensionAttribute, EncodingAttribute);
		}

	}

}
