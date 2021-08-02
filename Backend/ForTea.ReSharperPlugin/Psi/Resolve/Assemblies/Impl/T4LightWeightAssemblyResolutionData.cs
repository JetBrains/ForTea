using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
	public sealed class T4LightWeightAssemblyResolutionData
	{
		[NotNull]
		public IReadOnlyDictionary<string, VirtualFileSystemPath> ResolvedAssemblies { get; }

		public T4LightWeightAssemblyResolutionData(
			[NotNull] IReadOnlyDictionary<string, VirtualFileSystemPath> resolvedAssemblies
		) => ResolvedAssemblies = resolvedAssemblies;
	}
}
