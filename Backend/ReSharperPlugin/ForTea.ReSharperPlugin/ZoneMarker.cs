using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.VsIntegration.Zones;

namespace JetBrains.ForTea.ReSharperPlugin
{
  [ZoneMarker]
  public class ZoneMarker : IRequire<IT4Zone>, IRequire<IVisualStudioFrontendEnvZone>
  {
  }
}