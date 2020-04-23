using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Utils;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	/// <summary>
	/// PSI for T4 files might depend on other T4 files.
	/// To keep it up-to-date, we need to mark a file as dirty
	/// whenever anything it depends on is changed in any way.
	/// </summary>
	[SolutionComponent]
	public sealed class T4FileDependencyInvalidator
	{
		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> IndirectDependencies { get; set; } =
			new HashSet<IPsiSourceFile>();

		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> PreviousIterationIndirectDependencies { get; set; } =
			new HashSet<IPsiSourceFile>();

		public T4FileDependencyInvalidator(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state
		)
		{
			services.Files.ObserveAfterCommit(lifetime, TriggerDependencyInvalidation);
			state.IsInitialUpdateFinished.Change.Advise(lifetime, args =>
			{
				if (!args.HasNew || !args.New) return;
				TriggerDependencyInvalidation();
			});

			void TriggerDependencyInvalidation() => services.Locks.QueueOrExecute(
				lifetime,
				"T4 indirect dependencies invalidation",
				() =>
				{
					using var cookie = WriteLockCookie.Create();
					foreach (var file in IndirectDependencies)
					{
						file.SetBeingIndirectlyUpdated(true);
						services.Caches.MarkAsDirty(file);
						services.Files.MarkAsDirty(file);
					}

					foreach (var file in PreviousIterationIndirectDependencies.Except(IndirectDependencies))
					{
						file.SetBeingIndirectlyUpdated(false);
					}

					PreviousIterationIndirectDependencies = IndirectDependencies;
					IndirectDependencies = new HashSet<IPsiSourceFile>();
				}
			);

			notifier.OnFilesIndirectlyAffected.Advise(lifetime, files =>
			{
				services.Locks.AssertMainThread();
				foreach (var file in files)
				{
					IndirectDependencies.Add(file);
				}
			});
		}
	}
}
