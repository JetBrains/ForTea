using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
	[DaemonStage(StagesBefore = new[] {typeof(CollectUsagesStage)})]
	public class T4WarningStage : T4DaemonStageBase
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4WarningStage([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		protected override IDaemonStageProcess CreateProcess(
			IDaemonProcess process,
			IT4File file,
			IContextBoundSettingsStore settings
		) => new T4IncludeAwareDaemonProcess(file, process, Manager);
	}
}