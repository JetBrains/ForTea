using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
    private ISet<IPsiSourceFile> PreviousIterationIndirectDependencies { get; set; } =
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
      myLocks.ExecuteWithWriteLockWhenAvailable(Lifetime,
        $"{nameof(T4FileDependencyInvalidator)} :: AfterCommitSync",
        () =>
        {
          foreach (var file in indirectDependencies)
          {
            if (!file.IsValid()) return;
            file.SetBeingIndirectlyUpdated(true);
            Services.Caches.MarkAsDirty(file);
            Services.Files.MarkAsDirty(file);
          }

          foreach (var file in PreviousIterationIndirectDependencies.Except(indirectDependencies))
          {
            file.SetBeingIndirectlyUpdated(false);
          }

          PreviousIterationIndirectDependencies = indirectDependencies;
        });
    }

    protected override string ActivityName => "T4 indirect dependencies invalidation";
  }
}