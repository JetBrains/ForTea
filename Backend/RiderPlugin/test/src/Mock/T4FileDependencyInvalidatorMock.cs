using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Cache.Impl;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace JetBrains.ForTea.Tests.Mock
{
	[SolutionComponent]
	public sealed class T4FileDependencyInvalidatorMock : T4FileDependencyInvalidator
	{
		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> IndirectDependencies { get; } = new HashSet<IPsiSourceFile>();

		/// <summary>
		/// Guarantees that T4 file invalidation does not invalidate more than necessary
		/// and does not get stuck in an infinite loop
		/// </summary>
		private T4CommitStage CommitStage { get; set; }

		public T4FileDependencyInvalidatorMock(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state
		) : base(lifetime, notifier, services, state)
		{
		}

		protected override void QueueAfterCommit()
		{
			if (CommitStage == T4CommitStage.DependencyInvalidation) return;
			CommitStage = T4CommitStage.DependencyInvalidation;
			try
			{
				using (WriteLockCookie.Create())
				{
					// Filter just in case a miracle happens and the file gets deleted before being marked as dirty
					foreach (var file in IndirectDependencies.Where(file => file.IsValid()))
					{
						Services.Files.MarkAsDirty(file);
						Services.Caches.MarkAsDirty(file);
					}
				}

				IndirectDependencies.Clear();
				Services.Files.CommitAllDocuments();
			}
			finally
			{
				CommitStage = T4CommitStage.UserChangeApplication;
			}
		}

		protected override void OnFilesIndirectlyAffected(
			T4FileInvalidationData data,
			ISet<IPsiSourceFile> indirectDependencies
		)
		{
			if (CommitStage == T4CommitStage.DependencyInvalidation) return;
			// We want all files that were included before the update
			// and all the files that have become included now
			// to be updated, so we'll mark them as dirty later
			IndirectDependencies.AddRange(data.IndirectlyAffectedFiles);
		}
	}
}
