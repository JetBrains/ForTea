using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Macros
{
  [SolutionComponent(InstantiationEx.LegacyDefault)]
  public sealed class T4AfterMacroCacheBuiltFileInvalidator
  {
    private Lifetime Lifetime { get; }

    [NotNull] private List<IPsiSourceFile> FilesToInvalidate { get; set; } = new();

    [NotNull] private IPsiServices Services { get; }

    public T4AfterMacroCacheBuiltFileInvalidator(
      Lifetime lifetime,
      [NotNull] IPsiServices services,
      [NotNull] IPsiCachesState state,
      [NotNull] T4MacroResolutionCache cache
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
      cache.OnFileMarkedForInvalidation.Advise(lifetime, FilesToInvalidate.Add);
    }

    private void QueueAfterCommit() => Services.Locks.ExecuteOrQueue(Lifetime,
      "T4 file invalidation caused by a change in macros in that file", () =>
      {
        AfterCommitSync(FilesToInvalidate);
        FilesToInvalidate = new();
      });

    private void AfterCommitSync([NotNull] IEnumerable<IPsiSourceFile> filesToInvalidate)
    {
      using var cookie = WriteLockCookie.Create();
      foreach (var file in filesToInvalidate)
      {
        Services.Caches.MarkAsDirty(file);
        Services.Files.MarkAsDirty(file);
      }
    }
  }
}