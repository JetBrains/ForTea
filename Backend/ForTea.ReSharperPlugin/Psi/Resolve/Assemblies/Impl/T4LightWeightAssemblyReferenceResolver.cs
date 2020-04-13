using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public sealed class T4LightWeightAssemblyReferenceResolver : T4BasicLightWeightAssemblyReferenceResolver
	{
		[NotNull]
		private T4LightWeightAssemblyResolutionCache Cache { get; }

		public T4LightWeightAssemblyReferenceResolver([NotNull] T4LightWeightAssemblyResolutionCache cache) =>
			Cache = cache;

		public override FileSystemPath TryResolve(IT4PathWithMacros path)
		{
			string assemblyName = path.ResolveString();
			FileSystemPath result = null;
			Cache.Map.TryGetValue(path.ProjectFile.ToSourceFile().NotNull())?.ResolvedAssemblies
				.TryGetValue(assemblyName, out result);
			return result;
		}
	}
}
