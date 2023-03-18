using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
  [SolutionComponent]
  public class T4BasicLightWeightAssemblyReferenceResolver : IT4LightWeightAssemblyReferenceResolver
  {
    public virtual VirtualFileSystemPath TryResolve(T4ResolvedPath path) => null;
  }
}