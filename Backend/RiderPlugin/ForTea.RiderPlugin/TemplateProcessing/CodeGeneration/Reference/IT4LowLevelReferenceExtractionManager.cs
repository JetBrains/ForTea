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
    IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
      [NotNull, ItemNotNull] IList<VirtualFileSystemPath> directDependencies,
      [NotNull] IModuleReferenceResolveContext resolveContext
    );

    [CanBeNull]
    MetadataReference ResolveMetadata(Lifetime lifetime, [NotNull] VirtualFileSystemPath filePath);
  }
}