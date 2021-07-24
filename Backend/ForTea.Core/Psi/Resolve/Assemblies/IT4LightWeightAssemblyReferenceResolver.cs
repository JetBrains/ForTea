using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	public interface IT4LightWeightAssemblyReferenceResolver
	{
		[CanBeNull]
		VirtualFileSystemPath TryResolve([NotNull] T4ResolvedPath path);
	}
}
