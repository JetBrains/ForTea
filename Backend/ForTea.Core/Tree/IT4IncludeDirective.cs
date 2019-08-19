using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree
{
	public partial interface IT4IncludeDirective
	{
		bool Once { get; }

		[NotNull]
		IT4PathWithMacros Path { get; }
	}
}
