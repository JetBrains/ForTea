using GammaJul.ForTea.Core;
using JetBrains.Application.Environment;
using JetBrains.VsIntegration.Env;

namespace JetBrains.ForTea.ReSharperPlugin.Frontend;

[ZoneActivator]
public class ReSharperT4PluginActivatorVs(VisualStudioOutOfProcessMode myOopMode) : IActivateDynamic<IT4Zone>
{
  bool IActivateDynamic<IT4Zone>.ActivatorEnabled() => !myOopMode.IsOutOfProcess;
}
