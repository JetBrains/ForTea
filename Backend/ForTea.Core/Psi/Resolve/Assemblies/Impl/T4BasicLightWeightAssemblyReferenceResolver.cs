using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public class T4BasicLightWeightAssemblyReferenceResolver : IT4LightWeightAssemblyReferenceResolver
	{
		public virtual FileSystemPath TryResolve(IT4PathWithMacros path) => null;
	}
}
