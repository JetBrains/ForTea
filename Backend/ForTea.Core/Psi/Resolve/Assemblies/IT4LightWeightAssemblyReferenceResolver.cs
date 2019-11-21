using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	public interface IT4LightWeightAssemblyReferenceResolver
	{
		[CanBeNull]
		FileSystemPath TryResolve([NotNull] IProjectFile file, [NotNull] string assemblyName);
	}
}
