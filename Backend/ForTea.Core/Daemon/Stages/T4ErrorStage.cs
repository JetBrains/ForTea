using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
	/// <summary>Daemon stage that creates processes for adding error and warning highlights.</summary>
	[DaemonStage(StagesBefore = new[] {typeof(CollectUsagesStage)})]
	public sealed class T4ErrorStage : T4DaemonStageBase
	{
		protected override IDaemonStageProcess CreateProcess(
			IDaemonProcess process,
			IT4File file,
			IContextBoundSettingsStore settings
		) => new T4ErrorProcess(file, process);
	}
}
