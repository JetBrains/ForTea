using JetBrains.Application.BuildScript;
using JetBrains.Application.BuildScript.Install;
using JetBrains.Application.BuildScript.Solution;
using JetBrains.Build;

namespace JetBrains.ForTea.Tests.BuildScript
{
  public class RequestAppConfigInstall
  {
    [BuildStep]
    public static InstallAppConfig[] Run(AllAssembliesOnEverything allass, ProductHomeDirArtifact homedir)
    {
      return InstallAppConfig.FromBuildScriptClassInThatProject<RequestAppConfigInstall>(allass, homedir);
    }
  }
}