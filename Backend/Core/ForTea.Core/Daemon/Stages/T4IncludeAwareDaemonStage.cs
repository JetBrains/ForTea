using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
  [DaemonStage(Instantiation.DemandAnyThreadSafe, StagesBefore = new[] { typeof(CollectUsagesStage) })]
  public sealed class T4IncludeAwareDaemonStage : T4DaemonStageBase
  {
    protected override IDaemonStageProcess CreateProcess(
      IDaemonProcess process,
      IT4File file,
      IContextBoundSettingsStore settings
    ) => new T4IncludeAwareDaemonProcess(file, process);
  }
}