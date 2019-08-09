using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree
{
	/// <summary>Represents a T4 include. This is not a directive, it contains path to the included file.</summary>
	public interface IT4Include : IT4TreeNode
	{
		bool Once { get; }

		[NotNull]
		IT4PathWithMacros Path { get; }
	}
}
