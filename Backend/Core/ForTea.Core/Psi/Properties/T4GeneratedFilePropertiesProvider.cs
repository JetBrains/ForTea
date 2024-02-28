using GammaJul.ForTea.Core.TemplateProcessing.Managing;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules.MsBuild;

namespace GammaJul.ForTea.Core.Psi.Properties
{
  [PsiSharedComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4GeneratedFilePropertiesProvider :
    IMsbuildGeneratedFilePropertiesProvider,
    IPsiSourceFilePropertiesProvider
  {
    public double Order => 1;

    public IPsiSourceFileProperties GetPsiProperties(
      IPsiSourceFileProperties prevProperties,
      IProject project,
      IProjectFile projectFile,
      IPsiSourceFile sourceFile
    )
    {
      if (!(sourceFile is IPsiSourceFileWithLocation psiSourceFileWithLocation)) return prevProperties;
      return GetPsiProperties(prevProperties, project, psiSourceFileWithLocation);
    }

    public IPsiSourceFileProperties GetPsiProperties(
      IPsiSourceFileProperties prevProperties,
      IProject project,
      IPsiSourceFileWithLocation sourceFile
    )
    {
      var projectFile = sourceFile.ToProjectFile();
      if (projectFile == null) return prevProperties;

      // Solution can actually be null for invalid files.
      // Access to the component has to be safe because this is a shell component,
      // while IT4TargetFileChecker is a solution component,
      // so accessing it directly can cause an exception
      // if the daemon requests properties for a file after solution has been closed

      // ReSharper disable once ConstantConditionalAccessQualifier
      var checker = sourceFile.GetSolution()?.TryGetComponent<IT4TargetFileChecker>();
      if (checker == null) return prevProperties;
      if (checker.IsPreprocessResult(projectFile)) return T4PreprocessResultProjectFileProperties.Instance;
      return prevProperties;
    }
  }
}