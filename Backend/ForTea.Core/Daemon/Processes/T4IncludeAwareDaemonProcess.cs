using System;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
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
			var psiSourceFile = File.LogicalPsiSourceFile;
			var solution = psiSourceFile.GetSolution();
			var projectFile = psiSourceFile.ToProjectFile();
			if (projectFile == null) return;
			var visitor = new T4IncludeAwareDaemonProcessVisitor(psiSourceFile);
			using (T4MacroResolveContextCookie.GetOrCreate(projectFile))
			{
				File.ProcessDescendants(visitor);
			}

			var relevantHighlightings = visitor
				.Highlightings
				.Where(info => info.Range.Document.GetPsiSourceFile(solution) == File.PhysicalPsiSourceFile);
			committer(new DaemonStageResult(relevantHighlightings.ToArray()));
		}
	}
}
