using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
	/// <summary>Daemon stage that creates processes for adding error and warning highlights.</summary>
	[DaemonStage(StagesBefore = new[] {typeof(CollectUsagesStage)})]
	public sealed class T4ErrorStage : T4DaemonStageBase
	{
		[NotNull]
		private IT4TemplateTypeProvider TemplateTypeProvider { get; }

		public T4ErrorStage([NotNull] IT4TemplateTypeProvider templateTypeProvider) =>
			TemplateTypeProvider = templateTypeProvider;

		protected override IDaemonStageProcess CreateProcess(
			IDaemonProcess process,
			IT4File file,
			IContextBoundSettingsStore settings
		) => new T4ErrorProcess(file, process, TemplateTypeProvider);
	}
}
