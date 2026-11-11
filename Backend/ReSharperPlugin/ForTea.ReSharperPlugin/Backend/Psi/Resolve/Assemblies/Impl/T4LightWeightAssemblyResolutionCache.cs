using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ForTea.ReSharperPlugin.AnyEnd;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin.Backend.Psi.Resolve.Assemblies.Impl
{
  [PsiComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4LightWeightAssemblyResolutionCache :
    T4PsiAwareCacheBase<T4LightWeightAssemblyResolutionRequest, T4LightWeightAssemblyResolutionData>
  {
    private readonly IT4VisualStudioInterop myVisualStudio;

    public T4LightWeightAssemblyResolutionCache(
      Lifetime lifetime,
      [NotNull] IShellLocks locks,
      [NotNull] IPersistentIndexManager persistentIndexManager,
      [NotNull] IT4VisualStudioInterop visualStudio
    ) : base(lifetime, locks, persistentIndexManager, T4LightWeightAssemblyResolutionDataMarshaller.Instance)
    {
      myVisualStudio = visualStudio;
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
      var response = new Dictionary<string, VirtualFileSystemPath>();
      var unresolved = new List<string>();
      foreach (var path in request.NotNull().AssembliesToResolve)
      {
        var absolute = path.TryResolveAbsolutePath();
        if (absolute != null)
          response[path.ResolvedPath] = absolute;
        else
          unresolved.Add(path.ResolvedPath);
      }

      if (unresolved.Count != 0)
      {
        foreach (var pair in ResolveInVisualStudio(projectFile, unresolved.ToArray()))
        {
          var path = ParseAbsolutePath(pair.Value);
          if (path != null)
            response[pair.Key] = path;
        }
      }

      base.Merge(sourceFile, new T4LightWeightAssemblyResolutionData(response));
    }

    [CanBeNull]
    public T4LightWeightAssemblyResolutionData TryGetValue(IPsiSourceFile sourceFile)
    {
      base.TryGetValue(sourceFile, out var value);
      return value;
    }

    [CanBeNull]
    public VirtualFileSystemPath ResolveWithoutCaching([NotNull] T4ResolvedPath path)
    {
      var absolute = path.TryResolveAbsolutePath();
      if (absolute != null) return absolute;

      var resolved = ResolveInVisualStudio(path.ProjectFile, new[] { path.ResolvedPath });
      return resolved.TryGetValue(path.ResolvedPath, out var value) ? ParseAbsolutePath(value) : null;
    }

    private Dictionary<string, string> ResolveInVisualStudio(IProjectFile file, string[] references) =>
      myVisualStudio.ResolveAssemblies(
        file.GetProject().NotNull().ToProjectSearchDescriptor(),
        file.Location.IsNullOrEmpty() ? null : file.Location.FullPath, references);

    [CanBeNull]
    private static VirtualFileSystemPath ParseAbsolutePath(string value)
    {
      var path = VirtualFileSystemPath.TryParse(value, InteractionContext.SolutionContext);
      return !path.IsEmpty && path.IsAbsolute ? path : null;
    }
  }
}
