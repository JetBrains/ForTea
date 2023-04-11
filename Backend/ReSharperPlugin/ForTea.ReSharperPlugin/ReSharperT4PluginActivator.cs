using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.Platform.VisualStudio.SinceVs10.Zones;

#pragma warning disable CheckNamespace

namespace JetBrains.ForTea.ReSharperPluginActivation
{
  [ZoneActivator]
  [ZoneMarker(typeof(ISinceVs10FrontEnvZone))]
  public class ReSharperT4PluginActivator : IActivate<IT4Zone>
  {
  }
}