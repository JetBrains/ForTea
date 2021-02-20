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

		public override FileSystemPath Resolve(T4ResolvedPath pathWithMacros) =>
			base.Resolve(pathWithMacros) ?? ResolveAsDte(pathWithMacros.ResolvedPath);

		[CanBeNull]
		private static FileSystemPath ResolveAsDte([NotNull] string assemblyName) =>
			NameToEnvDTEAssemblyMap.TryGetValue(assemblyName);

		private static IDictionary<string, FileSystemPath> NameToEnvDTEAssemblyMap { get; } = FileSystemPath
			.Parse(typeof(Lifetime).Assembly.Location)
			.Parent
			.GetChildren("*EnvDTE*.dll")
			.Select(child => child.GetAbsolutePath())
			.ToDictionary(assembly => assembly.NameWithoutExtension);
	}
}
