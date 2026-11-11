using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.RdFramework;
using JetBrains.Platform.VisualStudio.AnyEnd.Protocol.Zones;

namespace JetBrains.ForTea.ReSharperPlugin.Backend
{
  [ZoneMarker]
  public class ZoneMarker : IRequire<IT4Zone>, IRequire<IVisualStudioBackendZone>, IRequire<IRdFrameworkZone>
  {
  }
}
