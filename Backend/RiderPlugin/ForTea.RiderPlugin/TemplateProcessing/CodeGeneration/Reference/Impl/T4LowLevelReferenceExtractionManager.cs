using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.Annotations;
using JetBrains.Application.Infra;
using JetBrains.Application.Parts;
using JetBrains.Diagnostics;
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
  [SolutionComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4LowLevelReferenceExtractionManager : IT4LowLevelReferenceExtractionManager
  {
    [NotNull] private Dictionary<string, WeakReference<MetadataReference>> MetadataReferencesCache { get; }

    [NotNull] private AssemblyInfoDatabase AssemblyInfoDatabase { get; }

    [NotNull] private ISolution Solution { get; }

    public T4LowLevelReferenceExtractionManager(
      Lifetime lifetime,
      [NotNull] AssemblyInfoDatabase assemblyInfoDatabase,
      [NotNull] ISolution solution
    )
    {
      AssemblyInfoDatabase = assemblyInfoDatabase;
      Solution = solution;
      MetadataReferencesCache = new Dictionary<string, WeakReference<MetadataReference>>();
    }

    // TODO: is this necessary?
    [NotNull]
    private static IAssemblyResolver ProtocolAssemblyResolver { get; } = new AssemblyResolverOnFolders(
      typeof(Lifetime).Assembly.GetPath().ToVirtualFileSystemPath().Parent.Parent, // JetBrains.Lifetimes on .net core
      typeof(IProtocol).Assembly.GetPath().ToVirtualFileSystemPath().Parent
        .Parent, // JetBrains.RdFramework on .net core
      typeof(Lifetime).Assembly.GetPath().ToVirtualFileSystemPath().Parent, // JetBrains.Lifetimes
      typeof(IProtocol).Assembly.GetPath().ToVirtualFileSystemPath().Parent // JetBrains.RdFramework
    );

    public IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
      IList<VirtualFileSystemPath> directDependencies,
      IModuleReferenceResolveContext resolveContext
    )
    {
      var result = new List<T4AssemblyReferenceInfo>();
      ResolveTransitiveDependencies(directDependencies.SelectNotNull(Resolve), resolveContext, result);
      return result;
    }

    private T4AssemblyReferenceInfo? Resolve([NotNull] VirtualFileSystemPath path)
    {
      var info = AssemblyInfoDatabase.GetAssemblyName(path);
      if (info == null) return null;
      return new T4AssemblyReferenceInfo(info.FullName, path);
    }

    public MetadataReference ResolveMetadata(Lifetime lifetime, VirtualFileSystemPath filePath)
    {
      var path = filePath.ToNativeFileSystemPath().FullPath;

      if (MetadataReferencesCache.TryGetValue(path, out var weakRef) &&
          weakRef.TryGetTarget(out var cached))
        return cached;

      var reference = MetadataReference.CreateFromFile(path,
        new MetadataReferenceProperties(kind: MetadataImageKind.Assembly));

      MetadataReferencesCache[path] = new WeakReference<MetadataReference>(reference);

      return reference;
    }

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
          .GetReferencedAssemblyNames(new AssemblyLocation(directDependency.Location))
          .SelectNotNull<AssemblyNameInfo, T4AssemblyReferenceInfo>(
            assemblyNameInfo =>
            {
              var resolver = BuildResolver(directDependency);
              resolver.ResolveAssembly(assemblyNameInfo, out var path, resolveContext);
              if (path == null)
              {
                var assemblyFromSolution = Solution.GetAllAssemblies()
                  .FirstOrDefault(assembly => assembly.AssemblyName == assemblyNameInfo);
                if (assemblyFromSolution == null) return null;
                return new T4AssemblyReferenceInfo(
                  assemblyFromSolution.FullAssemblyName,
                  assemblyFromSolution.Location.AssemblyPhysicalPath.NotNull()
                );
              }

              return new T4AssemblyReferenceInfo(assemblyNameInfo.FullName, path.AssemblyPhysicalPath.NotNull());
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