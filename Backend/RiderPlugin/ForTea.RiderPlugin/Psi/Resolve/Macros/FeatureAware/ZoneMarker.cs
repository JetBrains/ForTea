using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Rider.Backend.Env;
using JetBrains.Rider.Backend.Product;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.FeatureAware
{
  [ZoneMarker]
  public class ZoneMarker : IRequire<IRiderFeatureZone>, IRequire<IRiderProductEnvironmentZone>
  {
  }
}