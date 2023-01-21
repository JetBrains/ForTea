using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.Util;
using JetBrains.VsIntegration.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
  /// <summary>
  /// In R#, it is only possible to perform T4-specific assembly resolution on the main thread,
  /// so we have to cache them to be able to access them from the daemon
  /// </summary>
  [PsiComponent]
  public sealed class T4LightWeightAssemblyResolutionCache :
    T4PsiAwareCacheBase<T4LightWeightAssemblyResolutionRequest, T4LightWeightAssemblyResolutionData>
  {
    [NotNull] private readonly Lazy<Optional<ITextTemplatingComponents>> Components;

    public T4LightWeightAssemblyResolutionCache(
      Lifetime lifetime,
      [NotNull] IShellLocks locks,
      [NotNull] IPersistentIndexManager persistentIndexManager,
      [NotNull] RawVsServiceProvider provider
    ) : base(lifetime, locks, persistentIndexManager, T4LightWeightAssemblyResolutionDataMarshaller.Instance)
    {
      Components = Lazy.Of(() => new Optional<ITextTemplatingComponents>(
        provider.Value.GetService<STextTemplating, ITextTemplatingComponents>()
      ), true);
    }

    [NotNull]
    protected override T4LightWeightAssemblyResolutionRequest Build(IT4File file)
    {
      var assembliesToResolve = file
        .GetThisAndChildrenOfType<IT4AssemblyDirective>()
        .Select(directive => directive.ResolvedPath)
        .Distinct();
      return new T4LightWeightAssemblyResolutionRequest(assembliesToResolve);
    }

    public override void Merge(IPsiSourceFile sourceFile, object builtPart)
    {
      var request = (T4LightWeightAssemblyResolutionRequest)builtPart;
      var projectFile = sourceFile.ToProjectFile().NotNull();
      using var _ = Prepare(projectFile);
      var response = new Dictionary<string, VirtualFileSystemPath>();
      foreach (var path in request.NotNull().AssembliesToResolve)
      {
        var resolved = TryResolve(path);
        if (resolved == null) continue;
        response[path.ResolvedPath] = resolved;
      }

      var data = new T4LightWeightAssemblyResolutionData(response);
      base.Merge(sourceFile, data);
    }

    [CanBeNull]
    public VirtualFileSystemPath ResolveWithoutCaching([NotNull] T4ResolvedPath path)
    {
      using var _ = Prepare(path.ProjectFile);
      return TryResolve(path);
    }

    [CanBeNull]
    private VirtualFileSystemPath TryResolve([NotNull] T4ResolvedPath resolvedPath)
    {
      var asAbsolute = resolvedPath.TryResolveAbsolutePath();
      if (asAbsolute != null) return asAbsolute;
      string resolved = Components.Value.CanBeNull?.Host?.ResolveAssemblyReference(resolvedPath.ResolvedPath);
      if (resolved == null) return null;
      var path = VirtualFileSystemPath.Parse(resolved, InteractionContext.SolutionContext);
      if (path.IsAbsolute) return path;
      return null;
    }

    [NotNull]
    private IDisposable Prepare([NotNull] IProjectFile file)
    {
      var hierarchy = T4ResolutionUtils.TryGetVsHierarchy(file);
      var components = Components.Value.CanBeNull;
      if (components == null) return Disposable.Empty;
      object oldHierarchy = components.Hierarchy;
      string oldInputFileName = components.InputFile;
      return Disposable.CreateBracket(
        () =>
        {
          components.Hierarchy = hierarchy;
          components.InputFile = file.Location.IsNullOrEmpty() ? null : file.Location.FullPath;
        },
        () =>
        {
          components.Hierarchy = oldHierarchy;
          components.InputFile = oldInputFileName;
        },
        false
      );
    }
  }
}