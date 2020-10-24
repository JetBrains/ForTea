using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;
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
			return file.GetPsiModule().GetResolveContextEx(file);
		}

		[NotNull]
		public static TargetFrameworkId SelectTargetFrameworkId(
			[NotNull] this IT4Environment t4Environment,
			[CanBeNull] TargetFrameworkId candidate,
			[NotNull] IProjectFile file
		)
		{
			var project = file.GetProject();
			if (project?.IsMiscFilesProject() != false) return UniversalModuleReferenceContext.Instance.TargetFramework;
			// Generated C# code contains references to CodeDom, and thus can only be compiled by classical .NET
			// TODO: generate code without CodeDom in .NET Core projects
			if (candidate != null && candidate.IsNetFramework) return candidate;
			return t4Environment.TargetFrameworkId;
		}
	}
}
