using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Html;

namespace GammaJul.ForTea.Core
{
  [ZoneDefinition]
  [ZoneDefinitionConfigurableFeature("T4", "Provides support for editing T4 (.tt) files.", IsInProductSection: false)]
  public interface IT4Zone : IZone
    , IRequire<ILanguageCSharpZone>
    , IRequire<ICodeEditingZone>
    , IRequire<DaemonZone>
    , IRequire<NavigationZone>
    , IRequire<IPsiLanguageZone>
  {
  }
}