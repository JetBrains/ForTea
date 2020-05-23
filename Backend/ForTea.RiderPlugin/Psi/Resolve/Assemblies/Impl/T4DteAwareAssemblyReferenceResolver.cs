using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using EnvDTE90a;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
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
		private static FileSystemPath ResolveAsDte([NotNull] string assemblyName)
		{
			var assembly = NameToEnvDTEAssemblyMap.TryGetValue(assemblyName);
			if (assembly == null) return null;
			return FileSystemPath.Parse(assembly.Location);
		}

		[NotNull, ItemNotNull]
		private static IEnumerable<Assembly> EnvDTEAssemblies { get; } = new[]
		{
			typeof(Solution).Assembly,
			typeof(Solution2).Assembly,
			typeof(Solution3).Assembly,
			typeof(Debugger4).Assembly,
			typeof(Solution4).Assembly
		};

		private static IDictionary<string, Assembly> NameToEnvDTEAssemblyMap { get; } = EnvDTEAssemblies
			.ToDictionary(assembly => assembly.FullName)
			.Concat(EnvDTEAssemblies
				.ToDictionary(assembly => assembly.FullName.Substring(0, assembly.FullName.IndexOf(',')))
			)
			.ToDictionary(pair => pair.Key, pair => pair.Value);
	}
}
