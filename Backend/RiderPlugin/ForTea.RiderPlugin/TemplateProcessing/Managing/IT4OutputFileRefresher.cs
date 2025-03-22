using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  public interface IT4OutputFileRefresher
  {
    void Refresh([NotNull] IProjectFile output);
  }
}