using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.VsIntegration.Zones;

namespace JetBrains.ForTea.ReSharperPlugin.Frontend;

[ZoneMarker]
public class ZoneMarker : IRequire<IVisualStudioFrontendEnvZone>;
