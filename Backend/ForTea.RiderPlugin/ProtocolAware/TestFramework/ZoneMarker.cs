using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Host.Env;
using JetBrains.ReSharper.Host.Product;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.TestFramework
{
	[ZoneMarker]
	public class ZoneMarker : IRequire<IRiderCoreTestingZone>, IRequire<IRiderProductEnvironmentZone>
	{
	}
}
