using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.MSBuild;
using JetBrains.ProjectModel.Properties;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros
{
	[SolutionComponent]
	public class T4MacroResolver : T4MacroResolverBase
	{
		[NotNull]
		protected ISolution Solution { get; }

		public T4MacroResolver(
			[NotNull] ISolution solution,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor
		) : base(preprocessor) => Solution = solution;

		public override IReadOnlyDictionary<string, string> ResolveHeavyMacros(
			IEnumerable<string> macros,
			IProjectFile file
		) => EmptyDictionary<string, string>.Instance;

		public override IReadOnlyDictionary<string, string> ResolveAllLightMacros(IProjectFile file) =>
			GetAllLightMacros(file);

		[NotNull]
		protected virtual Dictionary<string, string> GetAllLightMacros([NotNull] IProjectFile file)
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
			result.Add("Configuration", project.ProjectProperties.ActiveConfigurations.Configurations.Single().Name);
			result.Add("TargetDir",
				project.GetOutputFilePath(project.GetCurrentTargetFrameworkId()).Parent
					.FullPathWithTrailingPathSeparator());
			result.Add("ProjectDir", project.Location.FullPathWithTrailingPathSeparator());
			result.Add("ProjectFileName", project.ProjectFileLocation.Name);
			result.Add("ProjectName", project.Name);
			result.Add("ProjectPath", project.Location.FullPathWithTrailingPathSeparator());
			var intermediate = project.GetIntermidiateDirectories().FirstOrDefault();
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

		private static ISet<string> UnsupportedMacros { get; } =
			new JetHashSet<string>(CaseInsensitiveComparison.Comparer)
			{
				"DevEnvDir",
				"FrameworkDir",
				"FrameworkSDKDir",
				"FrameworkVersion",
				"FxCopDir",
				"PlatformShortName",
				"ProjectExt",
				"RemoteMachine",
				"SolutionExt",
				"TargetFileName",
				"TargetName",
				"TargetPath",
				"VCInstallDir",
				"VSInstallDir",
				"WebDeployPath",
				"WebDeployRoot"
			};

		public override bool IsSupported(IT4Macro macro)
		{
			string value = macro.RawAttributeValue?.GetText();
			if (value == null) return true;
			return !UnsupportedMacros.Contains(value);
		}
	}
}
