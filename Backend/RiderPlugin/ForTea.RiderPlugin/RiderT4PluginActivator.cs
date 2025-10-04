using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.Rider.Backend.Env;

#pragma warning disable CheckNamespace

namespace JetBrains.ForTea.RiderPluginActivator
{
  [ZoneActivator]
  [ZoneMarker(typeof(IRiderBackendFullFeatureEnvironmentZone))]
  public class RiderT4PluginActivator : IActivate<IT4Zone>
  {
  }
}