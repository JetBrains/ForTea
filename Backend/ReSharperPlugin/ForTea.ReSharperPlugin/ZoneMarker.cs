using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;

namespace JetBrains.ForTea.ReSharperPlugin
{
  [ZoneMarker]
  public class ZoneMarker : IRequire<IT4Zone>, IRequire<ISinceVs10FrontEnvZone>
  {
  }
}