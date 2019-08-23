using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.MSBuild;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Host.Features.Processes;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros
{
	[SolutionComponent]
	public class T4MacroResolver : T4MacroResolverBase
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private RiderProcessStartInfoEnvironment Environment { get; }

		public T4MacroResolver(
			[NotNull] ISolution solution,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] RiderProcessStartInfoEnvironment environment
		) : base(preprocessor)
		{
			Solution = solution;
			Environment = environment;
		}

		public sealed override IReadOnlyDictionary<string, string> Resolve(IEnumerable<string> _, IProjectFile file) =>
			ResolveInternal(file);

		protected virtual Dictionary<string, string> ResolveInternal([NotNull] IProjectFile file)
		{
			var result = new Dictionary<string, string>(CaseInsensitiveComparison.Comparer);
			AddBasicMacros(result);
			AddSolutionMacros(result);
			AddProjectMacros(file, result);
			AddPlatformMacros(result);
			return result;
		}

		private void AddPlatformMacros(Dictionary<string, string> result)
		{
			switch (Environment.Platform)
			{
				case PlatformUtil.Platform.Windows:
					result.Add("Platform", "Win32");
					break;
				case PlatformUtil.Platform.MacOsX:
					result.Add("Platform", "MacOsX");
					break;
				case PlatformUtil.Platform.Linux:
					result.Add("Platform", "Linux");
					break;
			}

			result.Add("PlatformShortName", "x64");
		}

		private void AddProjectMacros(IProjectFile file, Dictionary<string, string> result)
		{
			var project = file.GetProject();
			if (project == null) return;
			result.Add("Configuration", project.ProjectProperties.ActiveConfigurations.Configurations.Single().Name);
			result.Add("TargetDir", project.GetOutputFilePath(project.GetCurrentTargetFrameworkId()).Parent.FullPath);
			result.Add("ProjectDir", project.Location.FullPath);
			result.Add("ProjectFileName", project.ProjectFileLocation.Name);
			result.Add("ProjectName", project.Name);
			result.Add("ProjectPath", project.Location.FullPath);
			var intermediate = project.GetIntermidiateDirectories().FirstOrDefault();
			if (intermediate != null) result.Add("IntDir", intermediate.FullPath);
			AddMsBuildProjectProperties(result, project);
		}

		private static void AddMsBuildProjectProperties(Dictionary<string, string> result, IProject project)
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

		private void AddBasicMacros(Dictionary<string, string> result)
		{
			result.Add("SolutionDir", Solution.SolutionDirectory.FullPath);
			result.Add("SolutionName", Solution.Name);
			result.Add("SolutionPath", Solution.SolutionFilePath.FullPath);
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
