using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Managing
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  public interface IT4TargetFileChecker
  {
    bool IsPreprocessResult([NotNull] IProjectFile suspect);
    bool IsGenerationResult([NotNull] IProjectFile suspect);
    bool IsGeneratedFrom([NotNull] IProjectFile generated, [NotNull] IProjectFile source);
  }
}