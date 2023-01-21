using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros
{
  public interface IT4LightMacroResolver
  {
    [NotNull]
    Dictionary<string, string> ResolveAllLightMacros([NotNull] IProjectFile file);
  }
}