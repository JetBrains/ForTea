using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
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

		public override FileSystemPath TryResolve(IProjectFile file, string assemblyName)
		{
			FileSystemPath path = null;
			Cache.Map.TryGetValue(file.ToSourceFile().NotNull())?.ResolvedAssemblies
				.TryGetValue(assemblyName, out path);
			return path;
		}
	}
}
