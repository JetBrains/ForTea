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
		public HashSet<FileSystemPath> CommittedFilePaths { get; } = new HashSet<FileSystemPath>();

		[NotNull]
		private T4FileDependencyManager FileDependencyManager { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

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
			if (CommittedFilePaths.Count == 0) return;
			bool markedAsDirty;

			// Mark includers file as dirty if their included files have changed.
			using (WriteLockCookie.Create())
			{
				var includers = CommittedFilePaths
					.SelectMany(committedFilePath => FileDependencyManager.GetIncluders(committedFilePath))
					.Where(includer => !CommittedFilePaths.Contains(includer))
					.Distinct() // Avoid unnecessary work
					.SelectMany(includer => PsiServices.Solution.FindProjectItemsByLocation(includer))
					.OfType<IProjectFile>()
					.AsList();
				markedAsDirty = includers.Any();
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
