using System.Linq;
using GammaJul.ForTea.Core.Psi.Modules;
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

		[NotNull]
		private ILogger Logger { get; }

		public T4PsiFileSelector(
			[NotNull] T4OutsideSolutionSourceFileManager outsideSolutionManager,
			[NotNull] ILogger logger
		)
		{
			OutsideSolutionManager = outsideSolutionManager;
			Logger = logger;
		}

		public IPsiSourceFile FindMostSuitableFile(FileSystemPath path, IPsiSourceFile requester)
		{
			var psf = TryFindFileInSolution(path, requester);
			if (psf != null) return psf;
			if (path.ExistsFile) return OutsideSolutionManager.GetOrCreateSourceFile(path);
			return null;
		}

		[CanBeNull]
		private IPsiSourceFile TryFindFileInSolution([NotNull] FileSystemPath path, [NotNull] IPsiSourceFile requester)
		{
			if (path.IsEmpty) return null;
			var potentialProjectFiles = requester
				.GetSolution()
				.FindProjectItemsByLocation(path)
				.OfType<IProjectFile>()
				.AsList();
			var requesterPsiModule = requester.PsiModule;
			var targetFrameworkId = requesterPsiModule.GetT4TargetFrameworkId();
			IProjectFile correctBuildActionItem;
			if (targetFrameworkId == null)
			{
				correctBuildActionItem = null;
				Logger.Warn(
					"Requester has no target framework! Requester: {0}, requester module: {1}",
					requester.GetType(),
					requesterPsiModule.GetType()
				);
			}
			else
			{
				correctBuildActionItem = potentialProjectFiles
					.FirstOrDefault(file => file.Properties.GetBuildAction(targetFrameworkId) == BuildAction.NONE);
			}

			var projectFile = correctBuildActionItem ?? potentialProjectFiles.FirstOrDefault();
			if (projectFile == null) return null;
			var sourceFiles = projectFile.ToSourceFiles();
			var correctTargetFrameworkItem = sourceFiles
				.FirstOrDefault<object>(null, (_, file) => file.PsiModule.GetT4TargetFrameworkId() == targetFrameworkId);
			return correctTargetFrameworkItem ?? sourceFiles.FirstOrDefault();
		}
	}
}
