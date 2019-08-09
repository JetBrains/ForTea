using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	public class T4ProblemAnalyzerProcess : IDaemonStageProcess
	{
		[NotNull]
		private IT4File File { get; }

		public IDaemonProcess DaemonProcess { get; }

		[NotNull]
		private ElementProblemAnalyzerRegistrar Registrar { get; }

		[NotNull]
		private IContextBoundSettingsStore Settings { get; }

		public T4ProblemAnalyzerProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess,
			[NotNull] ElementProblemAnalyzerRegistrar registrar,
			IContextBoundSettingsStore settings
		)
		{
			File = file;
			DaemonProcess = daemonProcess;
			Registrar = registrar;
			Settings = settings;
		}

		public void Execute(Action<DaemonStageResult> committer)
		{
			var consumer = new FilteringHighlightingConsumer(DaemonProcess.SourceFile, File, Settings);
			File.ProcessThisAndDescendants(new Processor(this, consumer));
			committer.Invoke(new DaemonStageResult(consumer.Highlightings));
		}

		private void Process(ITreeNode element, IHighlightingConsumer context)
		{
			var analyzerRunKind = ElementProblemAnalyzerRunKind.FullDaemon;
			var interruptCheck = DaemonProcess.GetCheckForInterrupt();
			var elementProblemAnalyzerData =
				new ElementProblemAnalyzerData(File, Settings, analyzerRunKind, interruptCheck);
			var analyzerDispatcher = Registrar.CreateDispatcher(elementProblemAnalyzerData);
			analyzerDispatcher.Run(element, context);
		}

		private class Processor : IRecursiveElementProcessor
		{
			public Processor([NotNull] T4ProblemAnalyzerProcess daemonProcess, [NotNull] IHighlightingConsumer consumer)
			{
				Consumer = consumer;
				DaemonProcess = daemonProcess;
			}

			[NotNull]
			private T4ProblemAnalyzerProcess DaemonProcess { get; }

			[NotNull]
			private IHighlightingConsumer Consumer { get; }

			public bool InteriorShouldBeProcessed(ITreeNode node) => true;

			public void ProcessBeforeInterior(ITreeNode element)
			{
			}

			public void ProcessAfterInterior(ITreeNode element) => DaemonProcess.Process(element, Consumer);

			public bool ProcessingIsFinished
			{
				get
				{
					if (DaemonProcess.DaemonProcess.InterruptFlag)
						throw new OperationCanceledException();
					return false;
				}
			}
		}
	}
}
