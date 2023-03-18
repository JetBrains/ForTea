using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Psi.Modules
{
  public static class T4PsiModuleExtensions
  {
    [CanBeNull]
    public static TargetFrameworkId GetT4TargetFrameworkId([NotNull] this IPsiModule module)
    {
      if (!(module is IT4FilePsiModule t4Module)) return null;
      return t4Module.OriginalTargetFrameworkId;
    }
  }
}