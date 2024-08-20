using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;

namespace JetBrains.ForTea.Tests.Mock
{
  [PsiComponent(InstantiationEx.LegacyDefault)]
  public sealed class T4TestsDeclaredAssembliesManager : T4DeclaredAssembliesManager
  {
    public T4TestsDeclaredAssembliesManager(
      Lifetime lifetime,
      [NotNull] IPsiFiles psiFiles,
      [NotNull] IShellLocks locks
    ) : base(lifetime, psiFiles, locks)
    {
    }

    protected override void CreateOrUpdateData(IT4File t4File) => DoInvalidateAssemblies(t4File);
  }
}