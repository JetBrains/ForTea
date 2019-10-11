using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.Platform.MsBuildHost.ProjectModel;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.ProjectsHost;
using JetBrains.ProjectModel.ProjectsHost.MsBuild;
using JetBrains.ProjectModel.ProjectsHost.SolutionHost;
using JetBrains.ReSharper.Host.Features.Processes;
using JetBrains.ReSharper.Host.Features.Toolset.Detecting;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.FeatureAware
{
	[SolutionComponent]
	public sealed class T4FeatureAwareMacroResolver : T4MacroResolver
	{
		[NotNull]
		private IBuildToolWellKnownPropertiesStore MsBuildProperties { get; }

		[NotNull]
		private ISolutionToolset SolutionToolset { get; }

		[NotNull]
		private RiderProcessStartInfoEnvironment Environment { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4FeatureAwareMacroResolver(
			[NotNull] ISolution solution,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] ISolutionToolset solutionToolset,
			[NotNull] IBuildToolWellKnownPropertiesStore msBuildProperties,
			[NotNull] RiderProcessStartInfoEnvironment environment,
			[NotNull] ILogger logger
		) : base(solution)
		{
			MsBuildProperties = msBuildProperties;
			SolutionToolset = solutionToolset;
			Environment = environment;
			Logger = logger;
		}

		protected override IReadOnlyDictionary<string, string> ResolveOnlyHeavyMacros(
			IList<string> heavyMacros,
			IProjectFile file
		)
		{
			Logger.Warn("Resolving {0} heavy (msbuild) macros", heavyMacros.Count);
			var mark = file.GetProject()?.GetProjectMark();
			if (mark == null) return EmptyDictionary<string, string>.Instance;
			var projectsHostContainer = Solution.ProjectsHostContainer();
			var msBuildSessionHolder = projectsHostContainer.GetComponent<MsBuildSessionHolder>();
			var msBuildSession = msBuildSessionHolder.Session;
			var result = new Dictionary<string, string>();
			foreach (string heavyMacro in heavyMacros)
			{
				string value = msBuildSession.GetProjectProperty(mark, heavyMacro)?.EvaluatedValue;
				if (value == null) continue;
				result.Add(heavyMacro, value);
			}

			return result;
		}

		protected override Dictionary<string, string> ResolveAllLightMacrosInternal(IProjectFile file)
		{
			var result = base.ResolveAllLightMacrosInternal(file);
			AddMsBuildMacros(result);
			AddPlatformMacros(result);
			return result;
		}

		private void AddPlatformMacros([NotNull] Dictionary<string, string> result)
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

		private void AddMsBuildMacros(Dictionary<string, string> result)
		{
			var buildTool = SolutionToolset.GetBuildTool();
			if (buildTool == null) return;
			var container = MsBuildProperties.Get(buildTool);
			if (container == null) return;
			foreach (string macro in container.GetKeys())
			{
				string value = container.GetValues(macro).FirstOrDefault();
				if (value == null) continue;
				result.Add(macro, value);
			}
		}
	}
}
