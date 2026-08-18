using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Parts;
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
  [SolutionComponent(InstantiationEx.LegacyDefault)]
  public class T4FileDependencyInvalidator : T4IndirectFileChangeObserverBase
  {
    [NotNull] private readonly IShellLocks myLocks;

    [NotNull, ItemNotNull]
    private readonly Queue<ISet<IPsiSourceFile>> myPendingIndirectDependencies = new();

    private bool myInvalidationScheduled;

    [NotNull, ItemNotNull]
    private ISet<IPsiSourceFile> PreviousIterationIndirectDependencies { get; } =
      new HashSet<IPsiSourceFile>();

    public T4FileDependencyInvalidator(
      Lifetime lifetime,
      [NotNull] IT4FileGraphNotifier notifier,
      [NotNull] IPsiServices services,
      [NotNull] IPsiCachesState state,
      [NotNull] IShellLocks locks
    ) : base(lifetime, notifier, services, state)
    {
      myLocks = locks;
    }

    protected sealed override void AfterCommitSync(ISet<IPsiSourceFile> indirectDependencies)
    {
      myLocks.AssertMainThread();

      if (indirectDependencies.Count == 0 &&
          PreviousIterationIndirectDependencies.Count == 0 &&
          myPendingIndirectDependencies.Count == 0 &&
          !myInvalidationScheduled)
        return;

      myPendingIndirectDependencies.Enqueue(indirectDependencies);
      ScheduleInvalidation();
    }

    private void ScheduleInvalidation()
    {
      if (myInvalidationScheduled)
        return;

      myInvalidationScheduled = true;
      myLocks.ExecuteWithWriteLockOrQueueAsync(Lifetime,
        $"{nameof(T4FileDependencyInvalidator)} :: AfterCommitSync",
        ProcessPendingInvalidations);
    }

    private void ProcessPendingInvalidations()
    {
      try
      {
        var stopwatch = Stopwatch.StartNew();
        while (myPendingIndirectDependencies.Count > 0 &&
               stopwatch.ElapsedMilliseconds < Interruption.InterruptionHandler.AcceptableTimeBetweenInterruptsMs)
        {
          var indirectDependencies = myPendingIndirectDependencies.Dequeue();
          var validIndirectDependencies = new HashSet<IPsiSourceFile>();
          foreach (var file in indirectDependencies)
          {
            if (!file.IsValid()) continue;
            validIndirectDependencies.Add(file);
            PreviousIterationIndirectDependencies.Add(file);
            file.SetBeingIndirectlyUpdated(true);
            Services.Caches.MarkAsDirty(file);
            Services.Files.MarkAsDirty(file);
          }

          foreach (var file in PreviousIterationIndirectDependencies.Except(validIndirectDependencies).ToList())
          {
            if (file.IsValid())
              file.SetBeingIndirectlyUpdated(false);
            PreviousIterationIndirectDependencies.Remove(file);
          }
        }
      }
      finally
      {
        myInvalidationScheduled = false;
        if (myPendingIndirectDependencies.Count > 0)
          myLocks.ExecuteOrQueue(Lifetime,
            $"{nameof(T4FileDependencyInvalidator)} :: RescheduleInvalidation",
            ScheduleInvalidation);
      }
    }

    protected override string ActivityName => "T4 indirect dependencies invalidation";
  }
}