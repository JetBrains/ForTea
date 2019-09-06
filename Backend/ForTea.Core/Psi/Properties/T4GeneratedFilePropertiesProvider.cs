using GammaJul.ForTea.Core.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules.MsBuild;

namespace GammaJul.ForTea.Core.Psi.Properties
{
	[PsiSharedComponent]
	public class T4GeneratedFilePropertiesProvider :
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
			var checker = sourceFile.GetSolution().GetComponent<IT4TargetFileChecker>();
			if (checker.IsPreprocessResult(projectFile)) return T4GeneratedProjectFileProperties.Instance;
			if (checker.IsGenerationResult(projectFile)) return T4GeneratedProjectFileProperties.Instance;
			return prevProperties;
		}
	}
}
