using JetBrains.Annotations;
using JetBrains.DataFlow;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  public interface IT4FileGraphNotifier
  {
    [NotNull] Signal<T4FileInvalidationData> OnFilesIndirectlyAffected { get; }
  }
}