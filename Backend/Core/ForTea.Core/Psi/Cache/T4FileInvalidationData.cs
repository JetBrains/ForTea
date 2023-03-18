using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  public readonly struct T4FileInvalidationData
  {
    [NotNull] public IEnumerable<IPsiSourceFile> IndirectlyAffectedFiles { get; }

    [NotNull] public IPsiSourceFile DirectlyAffectedFile { get; }

    public T4FileInvalidationData(
      [NotNull] IEnumerable<IPsiSourceFile> indirectlyAffectedFiles,
      [NotNull] IPsiSourceFile directlyAffectedFile
    )
    {
      IndirectlyAffectedFiles = indirectlyAffectedFiles;
      DirectlyAffectedFile = directlyAffectedFile;
    }
  }
}