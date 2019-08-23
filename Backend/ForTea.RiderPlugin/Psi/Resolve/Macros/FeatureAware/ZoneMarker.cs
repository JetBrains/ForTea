using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Host.Env;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.FeatureAware
{
	[ZoneMarker]
	public class ZoneMarker : IRequire<IRiderFeatureZone>
	{
	}
}
