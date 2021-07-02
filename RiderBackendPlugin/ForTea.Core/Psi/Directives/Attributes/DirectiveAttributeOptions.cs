using System;

namespace GammaJul.ForTea.Core.Psi.Directives.Attributes {

	[Flags]
	public enum DirectiveAttributeOptions {
		None = 0,
		Required = 1,
		DisplayInCodeStructure = 1 << 1
	}

}
