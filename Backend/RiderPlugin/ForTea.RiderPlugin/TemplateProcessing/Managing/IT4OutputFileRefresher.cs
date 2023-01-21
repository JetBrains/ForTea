using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
  public interface IT4OutputFileRefresher
  {
    void Refresh([NotNull] IProjectFile output);
  }
}