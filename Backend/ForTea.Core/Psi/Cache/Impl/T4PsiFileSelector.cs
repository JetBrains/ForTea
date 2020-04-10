using System.Collections.Generic;
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

		[CanBeNull]
		public IPsiSourceFile FindMostSuitableFile(FileSystemPath path, IPsiSourceFile requester)
		{
			var psf = PsiSourceFile(path, requester);
			if (psf != null) return psf;
			if (path.ExistsFile) return OutsideSolutionManager.GetOrCreateSourceFile(path);
			return null;
		}

		[CanBeNull]
		private static IPsiSourceFile PsiSourceFile([NotNull] FileSystemPath path, [NotNull] IPsiSourceFile requester)
		{
			if (path.IsEmpty) return null;
			var potentialProjectFiles = requester
				.GetSolution()
				.FindProjectItemsByLocation(path)
				.OfType<IProjectFile>()
				.AsList();
			var projectFile = SelectMostPlausibleT4ProjectFile(potentialProjectFiles, requester);
			if (projectFile == null) return null;
			return FindMostSuitableFile(projectFile, requester);
		}

		[CanBeNull]
		private static IProjectFile SelectMostPlausibleT4ProjectFile(
			[NotNull, ItemNotNull] IList<IProjectFile> files,
			[NotNull] IPsiSourceFile requester
		)
		{
			var targetFrameworkId = requester.PsiModule.TargetFrameworkId;
			var correctBuildActionItem = files
				.FirstOrDefault(file => file.Properties.GetBuildAction(targetFrameworkId) == BuildAction.NONE);
			return correctBuildActionItem ?? files.FirstOrDefault();
		}

		[CanBeNull]
		private static IPsiSourceFile FindMostSuitableFile(
			[NotNull] IProjectFile projectFile,
			[NotNull] IPsiSourceFile requester
		)
		{
			var sourceFiles = projectFile.ToSourceFiles();
			var targetFrameworkId = requester.PsiModule.TargetFrameworkId;
			var correctTargetFrameworkItem = sourceFiles
				.FirstOrDefault<object>(null, (_, file) => file.PsiModule.TargetFrameworkId == targetFrameworkId);
			return correctTargetFrameworkItem ?? sourceFiles.FirstOrDefault();
		}
	}
}
