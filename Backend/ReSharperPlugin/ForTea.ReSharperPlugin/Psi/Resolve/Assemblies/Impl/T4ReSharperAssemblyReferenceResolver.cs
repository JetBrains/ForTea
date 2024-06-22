using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
  [SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
  public sealed class T4ReSharperAssemblyReferenceResolver : T4AssemblyReferenceResolver
  {
    [NotNull] private T4LightWeightAssemblyResolutionCache Cache { get; }

    public T4ReSharperAssemblyReferenceResolver(
      [NotNull] IModuleReferenceResolveManager resolveManager,
      [NotNull] IT4LightWeightAssemblyReferenceResolver lightWeightResolver,
      [NotNull] T4LightWeightAssemblyResolutionCache cache
    ) : base(resolveManager, lightWeightResolver) => Cache = cache;

    public override VirtualFileSystemPath ResolveWithoutCaching(T4ResolvedPath path) =>
      Cache.ResolveWithoutCaching(path)
      ?? ResolveAsAssemblyName(path)
      ?? ResolveAsAssemblyFile(path);
  }
}