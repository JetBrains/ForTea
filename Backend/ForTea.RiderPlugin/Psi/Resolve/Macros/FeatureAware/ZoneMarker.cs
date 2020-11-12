using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Host.Env;
using JetBrains.ReSharper.Host.Product;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.FeatureAware
{
	[ZoneMarker]
	public class ZoneMarker : IRequire<IRiderFeatureZone>, IRequire<IRiderProductEnvironmentZone>
	{
	}
}
