using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Rider.Backend.Env;
using JetBrains.Rider.Backend.Product;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.TestFramework
{
	[ZoneMarker]
	public class ZoneMarker : IRequire<IRiderCoreTestingZone>, IRequire<IRiderProductEnvironmentZone>
	{
	}
}
