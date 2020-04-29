using System.Linq;
using GammaJul.ForTea.Core.Psi.OutsideSolution;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	[SolutionComponent]
	public sealed class T4PsiFileSelector : IT4PsiFileSelector
	{
		[NotNull]
		private T4OutsideSolutionSourceFileManager OutsideSolutionManager { get; }

		public T4PsiFileSelector([NotNull] T4OutsideSolutionSourceFileManager outsideSolutionManager) =>
			OutsideSolutionManager = outsideSolutionManager;

		public IPsiSourceFile FindMostSuitableFile(FileSystemPath path, IPsiSourceFile requester)
		{
			var psf = TryFindFileInSolution(path, requester);
			if (psf != null) return psf;
			if (path.ExistsFile) return OutsideSolutionManager.GetOrCreateSourceFile(path);
			return null;
		}

		[CanBeNull]
		private static IPsiSourceFile TryFindFileInSolution([NotNull] FileSystemPath path, [NotNull] IPsiSourceFile requester)
		{
			if (path.IsEmpty) return null;
			var potentialProjectFiles = requester
				.GetSolution()
				.FindProjectItemsByLocation(path)
				.OfType<IProjectFile>()
				.AsList();
			var targetFrameworkId = requester.PsiModule.TargetFrameworkId;
			var correctBuildActionItem = potentialProjectFiles
				.FirstOrDefault(file => file.Properties.GetBuildAction(targetFrameworkId) == BuildAction.NONE);
			var projectFile = correctBuildActionItem ?? potentialProjectFiles.FirstOrDefault();
			if (projectFile == null) return null;
			var sourceFiles = projectFile.ToSourceFiles();
			var correctTargetFrameworkItem = sourceFiles
				.FirstOrDefault<object>(null, (_, file) => file.PsiModule.TargetFrameworkId == targetFrameworkId);
			return correctTargetFrameworkItem ?? sourceFiles.FirstOrDefault();
		}
	}
}
