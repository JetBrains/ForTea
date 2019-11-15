using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	[SolutionComponent]
	public sealed class T4FileDependencyInvalidator
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> IndirectDependencies { get; } = new HashSet<IPsiSourceFile>();

		/// <summary>
		/// Guarantees that T4 file invalidation does not invalidate more than necessary
		/// and does not get stuck in an infinite loop
		/// </summary>
		private T4CommitStage CommitStage { get; set; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		public T4FileDependencyInvalidator(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] ISolution solution,
			[NotNull] IPsiServices psiServices
		)
		{
			Solution = solution;
			PsiServices = psiServices;
			services.Files.ObserveAfterCommit(lifetime, () =>
			{
				if (CommitStage == T4CommitStage.DependencyInvalidation) return;
				CommitStage = T4CommitStage.DependencyInvalidation;
				try
				{
					foreach (var file in IndirectDependencies)
					{
						services.Files.MarkAsDirty(file);
						services.Caches.MarkAsDirty(file);
					}

					IndirectDependencies.Clear();
					services.Files.CommitAllDocuments();
				}
				finally
				{
					CommitStage = T4CommitStage.UserChangeApplication;
				}
			});
			notifier.OnFilesIndirectlyAffected += paths =>
			{
				if (CommitStage == T4CommitStage.DependencyInvalidation) return;
				// We want all files that were included before the update
				// and all the files that have become included now
				// to be updated, so we'll mark them as dirty later
				IndirectDependencies.AddRange(paths
					.Distinct()
					.SelectMany(Solution.FindProjectItemsByLocation)
					.OfType<IProjectFile>()
					.Select(PsiServices.Modules.GetPsiSourceFilesFor)
					.SelectMany(sourceFiles => sourceFiles.AsEnumerable()));
			};
		}
	}
}
