using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  public interface IT4TemplateKindProvider
  {
    T4TemplateKind GetTemplateKind([CanBeNull] IPsiSourceFile file);
    T4TemplateKind GetTemplateKind([CanBeNull] IProjectFile file);
  }
}