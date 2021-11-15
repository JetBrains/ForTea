using GammaJul.ForTea.Core;

using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ForTea.TestsActivator;

namespace JetBrains.ForTea.Tests
{
	[ZoneMarker]
	public sealed class ZoneMarker : IRequire<IT4TestZone>
	{
	}
}