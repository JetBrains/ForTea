using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Documents;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting
{
	[SolutionComponent]
	public sealed class T4RiderSyntaxHighlightingHost
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private T4OutputExtensionFrontendNotifier Notifier { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IPsiCachesState State { get; }

		[NotNull]
		private IPsiFiles Files { get; }

		[NotNull]
		private DocumentHost Host { get; }

		public T4RiderSyntaxHighlightingHost(
			Lifetime lifetime,
			[NotNull] ILogger logger,
			[NotNull] T4OutputExtensionFrontendNotifier notifier,
			[NotNull] ISolution solution,
			[NotNull] IPsiCachesState state,
			[NotNull] IPsiFiles files,
			[NotNull] DocumentHost host
		)
		{
			Logger = logger;
			Notifier = notifier;
			Solution = solution;
			State = state;
			Files = files;
			Host = host;
			host.ViewHostDocuments(lifetime, CreateHandler);
		}

		private void CreateHandler(
			Lifetime editableEntityLifetime,
			[NotNull] EditableEntityId editableEntityId,
			[NotNull] RiderDocument document
		)
		{
			var editableEntity = Host.TryGetEditableEntity(editableEntityId);
			if (editableEntity == null)
			{
				Logger.Error("Editable entity not found in a document!");
				return;
			}

			var t4EditableEntityModel = editableEntity.GetT4EditableEntityModel();
			document.CreateOutputExtensionChangeListener(
				editableEntityLifetime,
				new T4OutputExtensionChangeListener(t4EditableEntityModel.RawTextExtension)
			);

			InitializeExtension(document);
		}

		private void InitializeExtension([NotNull] IDocument targetDocument)
		{
			var file = targetDocument.GetPsiSourceFile(Solution);
			if (file == null) return;
			// T4OutputExtensionFrontendNotifier handles initial update and invalidates all necessary files
			if (!State.IsInitialUpdateFinished.Value) return;
			Files.ExecuteAfterCommitAllDocuments(() => Notifier.NotifyFrontend(file));
		}
	}
}
