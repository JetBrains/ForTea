using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Processes;
using JetBrains.ReSharper.Host.Features.Toolset.Detecting;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.FeatureAware
{
	[SolutionComponent]
	public class T4FeatureAwareMacroResolver : T4MacroResolver
	{
		[NotNull]
		private IBuildToolWellKnownPropertiesStore MsBuildProperties { get; }

		[NotNull]
		private ISolutionToolset SolutionToolset { get; }

		[NotNull]
		private RiderProcessStartInfoEnvironment Environment { get; }
		
		public T4FeatureAwareMacroResolver(
			[NotNull] ISolution solution,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] ISolutionToolset solutionToolset,
			[NotNull] IBuildToolWellKnownPropertiesStore msBuildProperties,
			[NotNull] RiderProcessStartInfoEnvironment environment
		) : base(solution, preprocessor)
		{
			MsBuildProperties = msBuildProperties;
			SolutionToolset = solutionToolset;
			Environment = environment;
		}

		protected override Dictionary<string, string> ResolveInternal(IProjectFile file)
		{
			var result = base.ResolveInternal(file);
			AddMsBuildMacros(result);
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
		
		private void AddMsBuildMacros(Dictionary<string, string> result)
		{
			var buildTool = SolutionToolset.CurrentBuildTool;
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
