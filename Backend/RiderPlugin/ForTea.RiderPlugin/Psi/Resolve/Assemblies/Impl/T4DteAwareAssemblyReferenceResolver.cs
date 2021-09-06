using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Assemblies.Impl
{
	/// <summary>
	/// This resolver is capable of resolving everything its parent can,
	/// and it is additionally capable of resolving EnvDTE.
	/// </summary>
	[SolutionComponent]
	public sealed class T4DteAwareAssemblyReferenceResolver : T4AssemblyReferenceResolver
	{
		public T4DteAwareAssemblyReferenceResolver(
			[NotNull] IModuleReferenceResolveManager resolveManager,
			[NotNull] IT4LightWeightAssemblyReferenceResolver preprocessor
		) : base(
			resolveManager,
			preprocessor
		)
		{
		}

		public override VirtualFileSystemPath Resolve(T4ResolvedPath pathWithMacros) =>
			base.Resolve(pathWithMacros) ?? ResolveAsDte(pathWithMacros.ResolvedPath);

		[CanBeNull]
		private static VirtualFileSystemPath ResolveAsDte([NotNull] string assemblyName) =>
			NameToEnvDteAssemblyMap.TryGetValue(assemblyName);

		private static IDictionary<string, VirtualFileSystemPath> NameToEnvDteAssemblyMap { get; } =
			FindEnvDteAssemblies();

		private static Dictionary<string, VirtualFileSystemPath> FindEnvDteAssemblies()
		{
			var lifetimeDirectory = VirtualFileSystemPath
				.Parse(typeof(Lifetime).Assembly.Location, InteractionContext.SolutionContext)
				.Parent;
			var envDteAssembliesInLifetimeDirectory = FindEnvDteAssemblies(lifetimeDirectory);
			if (!envDteAssembliesInLifetimeDirectory.IsEmpty()) return envDteAssembliesInLifetimeDirectory;
			var envDteAssembliesInLifetimeDirectoryParent = FindEnvDteAssemblies(lifetimeDirectory.Parent);
			return envDteAssembliesInLifetimeDirectoryParent;
		}

		private static Dictionary<string, VirtualFileSystemPath> FindEnvDteAssemblies(
			VirtualFileSystemPath directory
		) => directory
			.GetChildren("*EnvDTE*.dll")
			.Select(child => child.GetAbsolutePath())
			.ToDictionary(assembly => assembly.NameWithoutExtension);
	}
}
