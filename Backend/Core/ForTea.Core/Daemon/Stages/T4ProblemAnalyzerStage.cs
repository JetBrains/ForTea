using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
  [DaemonStage]
  public class T4ProblemAnalyzerStage : T4DaemonStageBase
  {
    [NotNull] private ElementProblemAnalyzerRegistrar Registrar { get; }

    public T4ProblemAnalyzerStage([NotNull] ElementProblemAnalyzerRegistrar registrar) => Registrar = registrar;

    protected override IDaemonStageProcess CreateProcess(
      IDaemonProcess process,
      IT4File file,
      IContextBoundSettingsStore settings
    ) => new T4ProblemAnalyzerProcess(file, process, Registrar, settings);
  }
}