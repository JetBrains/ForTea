using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	public sealed class T4ProblemAnalyzerProcess : T4DaemonStageProcessBase
	{
		[NotNull]
		private IT4File File { get; }

		[NotNull]
		private ElementProblemAnalyzerRegistrar Registrar { get; }

		[NotNull]
		private IContextBoundSettingsStore Settings { get; }

		public T4ProblemAnalyzerProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess,
			[NotNull] ElementProblemAnalyzerRegistrar registrar,
			[NotNull] IContextBoundSettingsStore settings
		) : base(daemonProcess)
		{
			File = file;
			Registrar = registrar;
			Settings = settings;
		}

		protected override void DoExecute(Action<IEnumerable<HighlightingInfo>> committer)
		{
			var consumer = new FilteringHighlightingConsumer(DaemonProcess.SourceFile, File, Settings);
			File.ProcessThisAndDescendants(new Processor(this, consumer));
			committer.Invoke(consumer.Highlightings);
		}

		private void Process([NotNull] ITreeNode element, [NotNull] IHighlightingConsumer context)
		{
			var analyzerRunKind = ElementProblemAnalyzerRunKind.FullDaemon;
			var interruptCheck = DaemonProcess.GetCheckForInterrupt();
			var elementProblemAnalyzerData =
				new ElementProblemAnalyzerData(File, Settings, analyzerRunKind, interruptCheck);
			var analyzerDispatcher = Registrar.CreateDispatcher(elementProblemAnalyzerData);
			analyzerDispatcher.Run(element, context);
		}

		private sealed class Processor : IRecursiveElementProcessor
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
