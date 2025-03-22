using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  public interface IT4LightWeightAssemblyReferenceResolver
  {
    [CanBeNull]
    VirtualFileSystemPath TryResolve([NotNull] T4ResolvedPath path);
  }
}