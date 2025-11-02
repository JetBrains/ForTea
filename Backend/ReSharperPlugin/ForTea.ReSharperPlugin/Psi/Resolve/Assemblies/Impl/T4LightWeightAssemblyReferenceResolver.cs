using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
  [SolutionComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4LightWeightAssemblyReferenceResolver : T4BasicLightWeightAssemblyReferenceResolver
  {
    [NotNull] private T4LightWeightAssemblyResolutionCache Cache { get; }

    public T4LightWeightAssemblyReferenceResolver([NotNull] T4LightWeightAssemblyResolutionCache cache) =>
      Cache = cache;

    public override VirtualFileSystemPath TryResolve(T4ResolvedPath path)
    {
      VirtualFileSystemPath result = null;
      var resolutionData = Cache.TryGetValue(path.SourceFile);
      resolutionData?.ResolvedAssemblies.TryGetValue(path.ResolvedPath, out result);
      return result;
    }
  }
}