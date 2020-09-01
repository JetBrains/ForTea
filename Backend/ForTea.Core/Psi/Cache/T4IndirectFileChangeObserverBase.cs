using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public abstract class T4IndirectFileChangeObserverBase
	{
		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> IndirectDependencies { get; set; } = new HashSet<IPsiSourceFile>();

		[NotNull]
		protected IPsiServices Services { get; }

		private Lifetime Lifetime { get; }

		protected T4IndirectFileChangeObserverBase(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state
		)
		{
			Lifetime = lifetime;
			Services = services;
			services.Files.ObserveAfterCommit(lifetime, QueueAfterCommit);
			state.IsInitialUpdateFinished.Change.Advise(lifetime, args =>
			{
				if (!args.HasNew || !args.New) return;
				QueueAfterCommit();
			});
			notifier.OnFilesIndirectlyAffected.Advise(lifetime, data => OnFilesIndirectlyAffected(data, IndirectDependencies));
		}

		protected virtual void QueueAfterCommit() => Services.Locks.ExecuteOrQueue(Lifetime, ActivityName, () =>
		{
			AfterCommitSync(IndirectDependencies);
			IndirectDependencies = new HashSet<IPsiSourceFile>();
		});

		protected virtual void OnFilesIndirectlyAffected(
			T4FileInvalidationData data,
			[NotNull] ISet<IPsiSourceFile> indirectDependencies
		)
		{
			Services.Locks.AssertMainThread();
			foreach (var file in data.IndirectlyAffectedFiles)
			{
				IndirectDependencies.Add(file);
			}
		}

		[NotNull]
		protected abstract string ActivityName { get; }

		protected abstract void AfterCommitSync([NotNull] ISet<IPsiSourceFile> indirectDependencies);
	}
}
