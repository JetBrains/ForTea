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
		public static TargetFrameworkId SelectTargetFrameworkId([NotNull] this IProjectFile file)
		{
			var project = file.GetProject();
			if (project?.IsMiscFilesProject() != false) return UniversalModuleReferenceContext.Instance.TargetFramework;
			return project.GetResolveContext().TargetFramework;
		}
	}
}
