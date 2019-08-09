using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Host.Env;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	[ZoneMarker]
	public class ZoneMarker : IRequire<IRiderFeatureZone>
	{
	}
}
