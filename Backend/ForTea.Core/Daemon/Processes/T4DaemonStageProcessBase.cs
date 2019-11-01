using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	public abstract class T4DaemonStageProcessBase : IDaemonStageProcess
	{
		public IDaemonProcess DaemonProcess { get; }
		protected T4DaemonStageProcessBase([NotNull] IDaemonProcess daemonProcess) => DaemonProcess = daemonProcess;

		public void Execute(Action<DaemonStageResult> committer)
		{
			if (!DaemonProcess.SourceFile.IsValid()) return;
			DoExecute(committer);
		}

		protected abstract void DoExecute([NotNull] Action<DaemonStageResult> committer);
	}
}
