using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.Annotations;
using JetBrains.Application.Infra;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.Rd;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference.Impl
{
	[SolutionComponent]
	public class T4LowLevelReferenceExtractionManager : IT4LowLevelReferenceExtractionManager
	{
		[NotNull]
		private AssemblyInfoDatabase AssemblyInfoDatabase { get; }

		public T4LowLevelReferenceExtractionManager([NotNull] AssemblyInfoDatabase assemblyInfoDatabase) =>
			AssemblyInfoDatabase = assemblyInfoDatabase;

		[NotNull]
		private static IAssemblyResolver ProtocolAssemblyResolver { get; } = new AssemblyResolverOnFolders(
			FileSystemPath.Parse(typeof(Lifetime).Assembly.Location).Parent, // JetBrains.Lifetimes
			FileSystemPath.Parse(typeof(IProtocol).Assembly.Location).Parent // JetBrains.RdFramework
		);

		public IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
			IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			IModuleReferenceResolveContext resolveContext
		)
		{
			var result = new List<T4AssemblyReferenceInfo>();
			ResolveTransitiveDependencies(directDependencies, resolveContext, result);
			return result;
		}

		public IEnumerable<FileSystemPath> ResolveTransitiveDependencies(
			IList<FileSystemPath> directDependencies,
			IModuleReferenceResolveContext resolveContext
		) => ResolveTransitiveDependencies(ResolveAssemblies(directDependencies, resolveContext), resolveContext)
			.Select(it => it.Location)
			.Concat(directDependencies);

		public IEnumerable<T4AssemblyReferenceInfo> ResolveAssemblies(
			IEnumerable<FileSystemPath> directDependencies,
			IModuleReferenceResolveContext resolveContext
		) => directDependencies.SelectMany(
			directDependency => AssemblyInfoDatabase
				.GetReferencedAssemblyNames(directDependency)
				.SelectNotNull<AssemblyNameInfo, T4AssemblyReferenceInfo>(
					assemblyNameInfo =>
					{
						var resolver = new AssemblyResolverOnFolders(directDependency.Parent);
						resolver.ResolveAssembly(assemblyNameInfo, out var path, resolveContext);
						if (path == null) return null;
						return new T4AssemblyReferenceInfo(assemblyNameInfo.FullName, path);
					}
				)
		);

		private void ResolveTransitiveDependencies(
			[NotNull] IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext,
			[NotNull] IList<T4AssemblyReferenceInfo> destination
		)
		{
			foreach (var directDependency in directDependencies)
			{
				if (destination.Any(it => it.FullName == directDependency.FullName)) continue;
				destination.Add(directDependency);
				var indirectDependencies = AssemblyInfoDatabase
					.GetReferencedAssemblyNames(directDependency.Location)
					.SelectNotNull<AssemblyNameInfo, T4AssemblyReferenceInfo>(
						assemblyNameInfo =>
						{
							var resolver = BuildResolver(directDependency);
							resolver.ResolveAssembly(assemblyNameInfo, out var path, resolveContext);
							if (path == null) return null;
							return new T4AssemblyReferenceInfo(assemblyNameInfo.FullName, path);
						}
					);
				ResolveTransitiveDependencies(indirectDependencies, resolveContext, destination);
			}
		}

		private static IAssemblyResolver BuildResolver(T4AssemblyReferenceInfo directDependency) =>
			new CombiningAssemblyResolver(
				new AssemblyResolverOnFolders(directDependency.Location.Parent),
				ProtocolAssemblyResolver
			);
	}
}
