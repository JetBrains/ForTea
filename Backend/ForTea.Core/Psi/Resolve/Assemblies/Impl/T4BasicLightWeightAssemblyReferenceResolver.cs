using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public class T4BasicLightWeightAssemblyReferenceResolver : IT4LightWeightAssemblyReferenceResolver
	{
		public virtual FileSystemPath TryResolve(IProjectFile file, string assemblyName) => null;
	}
}
