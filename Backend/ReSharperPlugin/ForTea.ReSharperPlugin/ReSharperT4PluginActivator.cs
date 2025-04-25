using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.Platform.VisualStudio.Protocol.BuildScript;
using JetBrains.VsIntegration.Env;
using JetBrains.VsIntegration.Zones;

#pragma warning disable CheckNamespace

namespace JetBrains.ForTea.ReSharperPluginActivation.InProcess
{
  [ZoneActivator]
  [ZoneMarker(typeof(IVisualStudioFrontendEnvZone))]
  public class ReSharperT4PluginActivator(VisualStudioProtocolConnector protocolConnector) : IActivateDynamic<IT4Zone>
  {
    bool IActivateDynamic<IT4Zone>.ActivatorEnabled() => !protocolConnector.IsOutOfProcess;
  }
}

namespace JetBrains.ForTea.ReSharperPluginActivation.OutOfProcess
{
  [ZoneActivator]
  [ZoneMarker(typeof(IVisualStudioBackendOutOfProcessEnvZone))]
  public class ReSharperT4PluginActivator : IActivate<IT4Zone>;
}