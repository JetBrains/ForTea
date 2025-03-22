using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  public interface IT4PsiFileSelector
  {
    [CanBeNull]
    IPsiSourceFile FindMostSuitableFile(
      [NotNull] VirtualFileSystemPath path,
      [NotNull] IPsiSourceFile requester
    );
  }
}