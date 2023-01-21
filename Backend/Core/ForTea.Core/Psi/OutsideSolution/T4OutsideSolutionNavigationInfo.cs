using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
  public sealed class T4OutsideSolutionNavigationInfo
  {
    [NotNull] public VirtualFileSystemPath FileSystemPath { get; }
    public DocumentRange DocumentRange { get; }

    public T4OutsideSolutionNavigationInfo(
      [NotNull] VirtualFileSystemPath fileSystemPath,
      DocumentRange documentRange
    )
    {
      FileSystemPath = fileSystemPath;
      DocumentRange = documentRange;
    }
  }
}