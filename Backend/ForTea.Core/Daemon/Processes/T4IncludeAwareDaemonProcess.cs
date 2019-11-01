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
	public sealed class T4IncludeAwareDaemonProcess : T4DaemonStageProcessBase
	{
		[NotNull]
		private IT4File File { get; }

		public T4IncludeAwareDaemonProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess
		) : base(daemonProcess) => File = file;

		protected override void DoExecute(Action<DaemonStageResult> committer)
		{
			var psiSourceFile = File.LogicalPsiSourceFile;
			var projectFile = psiSourceFile.ToProjectFile();
			if (projectFile == null) return;
			var visitor = new T4IncludeAwareDaemonProcessVisitor(psiSourceFile);
			using (T4MacroResolveContextCookie.GetOrCreate(projectFile))
			{
				File.ProcessDescendants(visitor);
			}

			var relevantHighlightings = visitor
				.Highlightings
				// TODO: remove this line once @alexander.kirsanov adds similar filtering before adding highlightings
				// to the RangeableContainer
				.Where(info => info.Range.Document == File.PhysicalPsiSourceFile?.Document);
			committer(new DaemonStageResult(relevantHighlightings.ToArray()));
		}
	}
}
