using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Utils
{
  public sealed class T4IncludeGuard
  {
    [NotNull, ItemCanBeNull] private ISet<VirtualFileSystemPath> SeenFiles { get; }

    [NotNull, ItemCanBeNull] private Stack<VirtualFileSystemPath> FilesBeingProcessed { get; }

    public T4IncludeGuard()
    {
      FilesBeingProcessed = new Stack<VirtualFileSystemPath>();
      SeenFiles = new HashSet<VirtualFileSystemPath>();
    }

    public bool CanProcess([NotNull] VirtualFileSystemPath file) => !FilesBeingProcessed.Contains(file);

    public void StartProcessing([NotNull] VirtualFileSystemPath file)
    {
      FilesBeingProcessed.Push(file);
      SeenFiles.Add(file);
    }

    public bool HasSeenFile([NotNull] VirtualFileSystemPath file) => SeenFiles.Contains(file);
    public void EndProcessing() => FilesBeingProcessed.Pop();

    public void TryEndProcessing([CanBeNull] VirtualFileSystemPath file)
    {
      if (file == null) return;
      if (EqualityComparer<VirtualFileSystemPath>.Default.Equals(FilesBeingProcessed.Peek(), file))
        FilesBeingProcessed.Pop();
    }
  }
}