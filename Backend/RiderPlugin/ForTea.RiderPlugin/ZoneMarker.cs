using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.IDE.Debugger;
using JetBrains.RdBackend.Common.Env;

namespace JetBrains.ForTea.RiderPlugin
{
  [ZoneMarker]
  public class ZoneMarker : IRequire<IT4Zone>, IRequire<IReSharperHostCoreFeatureZone>
  {
  }
}