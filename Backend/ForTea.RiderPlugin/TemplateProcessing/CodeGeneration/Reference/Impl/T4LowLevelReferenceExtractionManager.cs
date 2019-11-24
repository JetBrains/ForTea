using System.Collections.Generic;
using System.Linq;
using Debugger.Common.MetadataAndPdb;
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
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference.Impl
{
	[SolutionComponent]
	public sealed class T4LowLevelReferenceExtractionManager : IT4LowLevelReferenceExtractionManager
	{
		[NotNull]
		private RoslynMetadataReferenceCache Cache { get; }

		[NotNull]
		private AssemblyInfoDatabase AssemblyInfoDatabase { get; }

		public T4LowLevelReferenceExtractionManager(
			Lifetime lifetime,
			[NotNull] AssemblyInfoDatabase assemblyInfoDatabase
		)
		{
			AssemblyInfoDatabase = assemblyInfoDatabase;
			Cache = new RoslynMetadataReferenceCache(lifetime);
		}

		// TODO: is this necessary?
		[NotNull]
		private static IAssemblyResolver ProtocolAssemblyResolver { get; } = new AssemblyResolverOnFolders(
			FileSystemPath.Parse(typeof(Lifetime).Assembly.Location).Parent, // JetBrains.Lifetimes
			FileSystemPath.Parse(typeof(IProtocol).Assembly.Location).Parent // JetBrains.RdFramework
		);

		public IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
			IList<FileSystemPath> directDependencies,
			IModuleReferenceResolveContext resolveContext
		)
		{
			var result = new List<T4AssemblyReferenceInfo>();
			ResolveTransitiveDependencies(directDependencies.SelectNotNull(Resolve), resolveContext, result);
			return result;
		}

		private T4AssemblyReferenceInfo? Resolve([NotNull] FileSystemPath path)
		{
			var info = AssemblyInfoDatabase.GetAssemblyName(path);
			if (info == null) return null;
			return new T4AssemblyReferenceInfo(info.FullName, path);
		}

		public MetadataReference ResolveMetadata(Lifetime lifetime, FileSystemPath path) =>
			Cache.GetMetadataReference(lifetime, path);

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

		[NotNull]
		private static IAssemblyResolver BuildResolver(T4AssemblyReferenceInfo directDependency) =>
			new CombiningAssemblyResolver(
				new AssemblyResolverOnFolders(directDependency.Location.Parent),
				ProtocolAssemblyResolver
			);
	}
}
