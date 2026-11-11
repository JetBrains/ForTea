using GammaJul.ForTea.Core;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.Platform.VisualStudio.AnyEnd.BuildScript;

namespace JetBrains.ForTea.ReSharperPlugin;

[ZoneActivator]
[ZoneMarker(typeof(IVisualStudioBackendOutOfProcessEnvZone))]
public class ReSharperT4PluginActivatorOop : IActivate<IT4Zone>;
