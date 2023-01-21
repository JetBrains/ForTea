using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
  public static class T4TemplateDataManagerExtensions
  {
    public static bool IsPreprocessedTemplate(
      [NotNull] this IT4TemplateKindProvider manager,
      [NotNull] IPsiSourceFile file
    ) => manager.GetTemplateKind(file) == T4TemplateKind.Preprocessed;

    public static bool IsPreprocessedTemplate(
      [NotNull] this IT4TemplateKindProvider manager,
      [NotNull] IProjectFile file
    ) => manager.GetTemplateKind(file) == T4TemplateKind.Preprocessed;

    public static bool IsRootPreprocessedTemplate(
      [NotNull] this IT4RootTemplateKindProvider manager,
      [NotNull] IPsiSourceFile file
    ) => manager.GetRootTemplateKind(file) == T4TemplateKind.Preprocessed;
  }
}