using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	public interface IT4LightWeightAssemblyReferenceResolver
	{
		[CanBeNull]
		FileSystemPath TryResolve([NotNull] IT4PathWithMacros path);
	}
}
