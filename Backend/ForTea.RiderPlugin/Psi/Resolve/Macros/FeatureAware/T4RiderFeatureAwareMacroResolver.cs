using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.Impl;
using JetBrains.Platform.MsBuildHost.ProjectModel;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.ProjectsHost;
using JetBrains.ProjectModel.ProjectsHost.MsBuild;
using JetBrains.ProjectModel.ProjectsHost.SolutionHost;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.FeatureAware
{
	[SolutionComponent]
	public sealed class T4RiderFeatureAwareMacroResolver : T4RiderMacroResolver
	{
		[NotNull]
		private ILogger Logger { get; }

		public T4RiderFeatureAwareMacroResolver(
			[NotNull] ISolution solution,
			[NotNull] ILogger logger,
			[NotNull] IT4LightMacroResolver lightMacroResolver
		) : base(solution, lightMacroResolver) => Logger = logger;

		public override IReadOnlyDictionary<string, string> ResolveHeavyMacros(
			IEnumerable<string> macros,
			IProjectFile file
		)
		{
			var requestedMacros = macros.ToList();
			var resolvedLightMacros = LightMacroResolver.ResolveAllLightMacros(file);
			var requestedHeavyMacros = requestedMacros.Where(it => !resolvedLightMacros.ContainsKey(it)).AsList();
			if (!requestedHeavyMacros.Any()) return resolvedLightMacros;
			var resolvedHeavyMacros = ResolveOnlyHeavyMacros(requestedHeavyMacros, file);
			resolvedLightMacros.AddRange(resolvedHeavyMacros);
			return resolvedLightMacros;
		}

		[NotNull]
		private IReadOnlyDictionary<string, string> ResolveOnlyHeavyMacros(
			[NotNull] IList<string> heavyMacros,
			[NotNull] IProjectFile file
		)
		{
			Logger.Verbose("Resolving {0} heavy (msbuild) macros", heavyMacros.Count);
			var project = file.GetProject();
			if ((project.GetProjectKind() & (ProjectKind.REGULAR_PROJECT | ProjectKind.WEB_SITE)) == 0)
			{
				return EmptyDictionary<string, string>.Instance;
			}

			var mark = project?.GetProjectMark();
			if (mark == null) return EmptyDictionary<string, string>.Instance;
			var projectsHostContainer = Solution.ProjectsHostContainer();
			var msBuildSessionHolder = projectsHostContainer.GetComponent<MsBuildSessionHolder>();
			var msBuildSession = msBuildSessionHolder.Session;
			var result = new Dictionary<string, string>();
			var currentTargetFrameworkId = project.GetCurrentTargetFrameworkId();
			foreach (string heavyMacro in heavyMacros)
			{
				string value = msBuildSession.GetProjectProperty(mark, heavyMacro, currentTargetFrameworkId);
				if (value.IsNullOrEmpty()) continue;
				result[heavyMacro] = value;
			}

			return result;
		}
	}
}
