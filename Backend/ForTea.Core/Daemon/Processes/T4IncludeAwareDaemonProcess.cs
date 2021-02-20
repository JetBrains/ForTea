using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	/// <summary>
	/// Performs some specific analysis that cannot be done through problem analyzers
	/// </summary>
	public sealed class T4IncludeAwareDaemonProcess : T4DaemonStageProcessBase
	{
		[NotNull]
		private IT4File File { get; }

		public T4IncludeAwareDaemonProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess
		) : base(daemonProcess) => File = file;

		protected override void DoExecute(Action<IEnumerable<HighlightingInfo>> committer)
		{
			var psiSourceFile = File.LogicalPsiSourceFile;
			if (!psiSourceFile.IsValid()) return;
			var projectFile = psiSourceFile.ToProjectFile();
			if (projectFile == null) return;
			var visitor = new T4IncludeAwareDaemonProcessVisitor(psiSourceFile);
			File.ProcessDescendants(visitor);
			committer(visitor.Highlightings);
		}
	}
}
