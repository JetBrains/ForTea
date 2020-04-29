using System.Collections.Generic;
using System.Linq;
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
	public class T4FileDependencyInvalidator
	{
		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> IndirectDependencies { get; set; } =
			new HashSet<IPsiSourceFile>();

		[NotNull, ItemNotNull]
		private ISet<IPsiSourceFile> PreviousIterationIndirectDependencies { get; set; } =
			new HashSet<IPsiSourceFile>();

		[NotNull]
		protected IPsiServices Services { get; }

		private Lifetime Lifetime { get; }

		public T4FileDependencyInvalidator(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state
		)
		{
			Lifetime = lifetime;
			Services = services;
			services.Files.ObserveAfterCommit(lifetime, AfterCommit);
			state.IsInitialUpdateFinished.Change.Advise(lifetime, args =>
			{
				if (!args.HasNew || !args.New) return;
				AfterCommit();
			});
			notifier.OnFilesIndirectlyAffected.Advise(lifetime, OnFilesIndirectlyAffected);
		}

		protected virtual void AfterCommit() =>
			Services.Locks.ExecuteOrQueue(Lifetime, "T4 indirect dependencies invalidation", () =>
			{
				using var cookie = WriteLockCookie.Create();
				foreach (var file in IndirectDependencies)
				{
					file.SetBeingIndirectlyUpdated(true);
					Services.Caches.MarkAsDirty(file);
					Services.Files.MarkAsDirty(file);
				}

				foreach (var file in PreviousIterationIndirectDependencies.Except(IndirectDependencies))
				{
					file.SetBeingIndirectlyUpdated(false);
				}

				PreviousIterationIndirectDependencies = IndirectDependencies;
				IndirectDependencies = new HashSet<IPsiSourceFile>();
			});

		protected virtual void OnFilesIndirectlyAffected([NotNull, ItemNotNull] IEnumerable<IPsiSourceFile> files)
		{
			Services.Locks.AssertMainThread();
			foreach (var file in files)
			{
				IndirectDependencies.Add(file);
			}
		}
	}
}
