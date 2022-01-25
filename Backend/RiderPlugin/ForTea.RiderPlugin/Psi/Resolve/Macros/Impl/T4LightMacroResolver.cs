using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.MSBuild;
using JetBrains.ProjectModel.Properties;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.Impl
{
	[SolutionComponent]
	public class T4LightMacroResolver : IT4LightMacroResolver
	{
		[NotNull]
		private ISolution Solution { get; }

		public T4LightMacroResolver([NotNull] ISolution solution) => Solution = solution;

		public virtual Dictionary<string, string> ResolveAllLightMacros(IProjectFile file)
		{
			var result = new Dictionary<string, string>(CaseInsensitiveComparison.Comparer);
			AddBasicMacros(result);
			AddProjectMacros(file, result);
			AddSolutionMacros(result);
			return result;
		}

		private void AddProjectMacros([NotNull] IProjectFile file, Dictionary<string, string> result)
		{
			var project = file.GetProject();
			if (project == null) return;
			var configuration = project.ProjectProperties.ActiveConfigurations.Configurations.SingleItem();
			if (configuration != null) result.Add("Configuration", configuration.Name);
			result.Add("TargetDir",
				project.GetOutputFilePath(project.GetCurrentTargetFrameworkId()).Parent
					.FullPathWithTrailingPathSeparator());
			result.Add("ProjectDir", project.Location.FullPathWithTrailingPathSeparator());
			result.Add("ProjectFileName", project.ProjectFileLocation.Name);
			result.Add("ProjectName", project.Name);
			result.Add("ProjectPath", project.Location.FullPathWithTrailingPathSeparator());
			var intermediate = project.GetIntermediateDirectories().FirstOrDefault();
			if (intermediate != null) result.Add("IntDir", intermediate.FullPathWithTrailingPathSeparator());
			AddMsBuildProjectProperties(result, project);
		}

		private static void AddMsBuildProjectProperties(
			[NotNull] Dictionary<string, string> result,
			[NotNull] IProject project
		)
		{
			result.Add("TargetExt", T4MSBuildProjectUtil.GetTargetExtension(project));
			string rootNamespace = project
				.GetRequestedProjectProperties(MSBuildProjectUtil.RootNamespaceProperty)
				.FirstOrDefault();
			if (rootNamespace != null) result.Add("RootNameSpace", rootNamespace);
			string outDir = project.GetRequestedProjectProperties(MSBuildProjectUtil.OutDirProperty).FirstOrDefault();
			if (outDir != null) result.Add("OutDir", outDir);
		}

		private void AddSolutionMacros(Dictionary<string, string> result)
		{
			var solutionFile = Solution.SolutionFile;
			if (solutionFile == null) return;
			result.Add("SolutionFileName", solutionFile.Name);
		}

		private void AddBasicMacros([NotNull] Dictionary<string, string> result)
		{
			result.Add("SolutionDir", Solution.SolutionDirectory.FullPathWithTrailingPathSeparator());
			result.Add("SolutionName", Solution.Name);
			result.Add("SolutionPath", Solution.SolutionFilePath.FullPathWithTrailingPathSeparator());
		}
	}
}
