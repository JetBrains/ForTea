using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  public interface IT4LightMacroResolver
  {
    [NotNull]
    Dictionary<string, string> ResolveAllLightMacros([NotNull] IProjectFile file);
  }
}