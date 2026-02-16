using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
  [DaemonStage(Instantiation.DemandAnyThreadUnsafe, StagesBefore = [typeof(GlobalFileStructureCollectorStage)])]
  public class T4ProblemAnalyzerStage([NotNull] ElementProblemAnalyzerRegistrar registrar)
    : T4DaemonStageBase
  {
    [NotNull] private ElementProblemAnalyzerRegistrar Registrar { get; } = registrar;

    protected override IDaemonStageProcess CreateProcess(
      IDaemonProcess process, IT4File file, IContextBoundSettingsStore settings)
    {
      return new T4ProblemAnalyzerProcess(file, process, Registrar, settings);
    }
  }
}