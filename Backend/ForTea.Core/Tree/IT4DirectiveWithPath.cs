using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Tree
{
	public interface IT4DirectiveWithPath
	{
		[NotNull]
		IProjectFile ResolutionContext { get; set; }

		[NotNull]
		IT4PathWithMacros Path { get; }
	}
}
