using System;
using System.Threading;
using GammaJul.ForTea.Core.Psi.Utils;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Parsing
{
  public static class T4ParsingContextHelper
  {
    [NotNull]
    private static ThreadLocal<T4IncludeGuard> Guard { get; } =
      new ThreadLocal<T4IncludeGuard>(() => new T4IncludeGuard());

    public static T ExecuteGuarded<T>([NotNull] VirtualFileSystemPath path, bool once, Func<T> action)
    {
      if (once && Guard.Value.HasSeenFile(path)) return default;
      if (!Guard.Value.CanProcess(path)) return default;
      Guard.Value.StartProcessing(path);
      try
      {
        return action();
      }
      finally
      {
        Guard.Value.EndProcessing();
      }
    }

    public static void Reset() => Guard.Value = new T4IncludeGuard();
  }
}