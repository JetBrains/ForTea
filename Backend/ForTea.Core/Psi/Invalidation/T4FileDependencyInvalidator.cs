using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation
{
	internal sealed class T4FileDependencyInvalidator
	{
		[NotNull, ItemNotNull]
		private HashSet<FileSystemPath> CommittedFilePaths { get; } = new HashSet<FileSystemPath>();

		[NotNull]
		private T4FileDependencyManager FileDependencyManager { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		[NotNull]
		private ILogger Logger { get; } = JetBrains.Util.Logging.Logger.GetLogger<T4FileDependencyInvalidator>();

		public T4FileDependencyInvalidator(
			[NotNull] T4FileDependencyManager fileDependencyManager,
			[NotNull] IPsiServices psiServices
		)
		{
			FileDependencyManager = fileDependencyManager;
			PsiServices = psiServices;
		}

		public void AddCommittedFilePath(FileSystemPath path) => CommittedFilePaths.Add(path);

		public void CommitNeededDocuments()
		{
			int committedFilesCount = CommittedFilePaths.Count;
			Logger.Verbose("{0} T4 files were committed during the current commit", committedFilesCount);
			if (committedFilesCount == 0) return;

			bool markedAsDirty;

			using (WriteLockCookie.Create())
			{
				var includers = CommittedFilePaths
					.SelectMany(FileDependencyManager.Graph.FindIndirectIncludesTransitiveClosure)
					.Distinct()
					.SelectMany(includer => PsiServices.Solution.FindProjectItemsByLocation(includer))
					.OfType<IProjectFile>()
					.AsList();
				markedAsDirty = includers.Any();
				Logger.Verbose("Marked {0} files as dirty because their dependencies changed", includers.Count);
				foreach (var includer in includers)
				{
					PsiServices.MarkAsDirty(includer);
				}
			}

			// Re-commit all documents again if needed, we need a clean state here.
			if (markedAsDirty) PsiServices.Files.CommitAllDocuments();
		}
	}
}
