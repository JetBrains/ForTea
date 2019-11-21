using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference
{
	public interface IT4LowLevelReferenceExtractionManager
	{
		[NotNull]
		IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
			[NotNull] IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext
		);

		[NotNull]
		IEnumerable<FileSystemPath> ResolveTransitiveDependencies(
			[NotNull, ItemNotNull] IList<FileSystemPath> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext
		);

		[NotNull]
		IEnumerable<T4AssemblyReferenceInfo> ResolveAssemblies(
			[NotNull] [ItemNotNull] IEnumerable<FileSystemPath> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext
		);
	}
}
