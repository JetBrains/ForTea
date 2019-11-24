using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference
{
	public interface IT4LowLevelReferenceExtractionManager
	{
		[NotNull]
		IEnumerable<FileSystemPath> ResolveTransitiveDependencies(
			[NotNull, ItemNotNull] IList<FileSystemPath> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext
		);

		T4AssemblyReferenceInfo? Resolve([NotNull] FileSystemPath path);

		[CanBeNull]
		MetadataReference ResolveMetadata(Lifetime lifetime, [NotNull] FileSystemPath path);
	}
}
