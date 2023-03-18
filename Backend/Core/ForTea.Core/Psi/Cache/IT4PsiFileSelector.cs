using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  public interface IT4PsiFileSelector
  {
    [CanBeNull]
    IPsiSourceFile FindMostSuitableFile(
      [NotNull] VirtualFileSystemPath path,
      [NotNull] IPsiSourceFile requester
    );
  }
}