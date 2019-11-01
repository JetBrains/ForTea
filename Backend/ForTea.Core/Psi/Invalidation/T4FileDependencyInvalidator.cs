using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
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
		private IT4FileDependencyGraph Graph { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		[NotNull]
		private T4DeclaredAssembliesManager DeclaresAssembliesManager { get; }

		[NotNull]
		private ILogger Logger { get; } = JetBrains.Util.Logging.Logger.GetLogger<T4FileDependencyInvalidator>();

		public T4FileDependencyInvalidator(
			[NotNull] IPsiServices psiServices,
			[NotNull] IT4FileDependencyGraph graph,
			[NotNull] T4DeclaredAssembliesManager declaresAssembliesManager
		)
		{
			PsiServices = psiServices;
			Graph = graph;
			DeclaresAssembliesManager = declaresAssembliesManager;
		}

		public void AddCommittedFilePath(FileSystemPath path) => CommittedFilePaths.Add(path);

		public void CommitNeededDocuments()
		{
			int committedFilesCount = CommittedFilePaths.Count;
			Logger.Verbose("{0} T4 files were committed during the current commit", committedFilesCount);
			if (committedFilesCount == 0) return;

			var targets = CommittedFilePaths
				.SelectMany(Graph.FindIndirectIncludesTransitiveClosure)
				.Distinct()
				.SelectMany(target => PsiServices.Solution.FindProjectItemsByLocation(target))
				.OfType<IProjectFile>()
				.AsList();
			using (WriteLockCookie.Create())
			{
				Logger.Verbose("Marked {0} files as dirty because their dependencies changed", targets.Count);
				foreach (var includer in targets)
				{
					PsiServices.MarkAsDirty(includer);
				}
			}

			// Re-commit all documents again if needed, we need a clean state here.
			if (targets.Any()) PsiServices.Files.CommitAllDocuments();
			DeclaresAssembliesManager.UpdateReferences(targets);
		}
	}
}
