using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Tree
{
	public interface IT4DirectiveWithPath : IT4TreeNode
	{
		[NotNull]
		IProjectFile ResolutionContext { get; set; }

		[NotNull]
		IT4PathWithMacros GetOrCreatePath([CanBeNull] IPsiSourceFile file = null);
	}
}
