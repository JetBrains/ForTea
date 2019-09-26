using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public static class T4ModulePsiUtils
	{
		[NotNull]
		public static IModuleReferenceResolveContext SelectResolveContext([NotNull] this IProjectFile file)
		{
			var project = file.GetProject();
			if (project?.IsMiscFilesProject() != false) return UniversalModuleReferenceContext.Instance;
			return project.GetResolveContext();
		}

		[NotNull]
		public static TargetFrameworkId SelectTargetFrameworkId(
			[NotNull] this IProjectFile file,
			[NotNull] IT4Environment t4Environment
		)
		{
			var project = file.GetProject();
			if (project?.IsMiscFilesProject() != false) return UniversalModuleReferenceContext.Instance.TargetFramework;
			var containingTargetFrameworkId = project.GetResolveContext().TargetFramework;
			if (containingTargetFrameworkId.IsNetFramework) return containingTargetFrameworkId;
			return t4Environment.TargetFrameworkId;
		}
	}
}
