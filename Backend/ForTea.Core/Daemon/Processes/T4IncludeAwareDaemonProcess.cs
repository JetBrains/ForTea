using System;
using System.Linq;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	/// <summary>
	/// Performs some specific analysis that cannot be done through problem analyzers
	/// </summary>
	public class T4IncludeAwareDaemonProcess : IDaemonStageProcess
	{
		[NotNull]
		private IT4File File { get; }

		public IDaemonProcess DaemonProcess { get; }

		public T4IncludeAwareDaemonProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess
		)
		{
			File = file;
			DaemonProcess = daemonProcess;
		}

		public void Execute(Action<DaemonStageResult> committer)
		{
			var visitor = new T4IncludeAwareDaemonProcessVisitor(File.GetSourceFile().NotNull());
			File.ProcessDescendants(visitor);
			committer(new DaemonStageResult(visitor.Highlightings.ToArray()));
		}
	}
}
