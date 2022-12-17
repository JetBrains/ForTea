using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.Documents;
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
		private IDocumentHost Host { get; }

		public T4RiderSyntaxHighlightingHost(
			Lifetime lifetime,
			[NotNull] ILogger logger,
			[NotNull] T4OutputExtensionFrontendNotifier notifier,
			[NotNull] ISolution solution,
			[NotNull] IPsiCachesState state,
			[NotNull] IPsiFiles files,
			[NotNull] IDocumentHost host
		)
		{
			Logger = logger;
			Notifier = notifier;
			Solution = solution;
			State = state;
			Files = files;
			Host = host;
			host.ViewDocuments(lifetime, CreateHandler);
		}

		private void CreateHandler(
			Lifetime editableEntityLifetime,
			[NotNull] RdDocumentId rdDocumentId,
			[NotNull] RiderDocument document, RdDocumentModel arg4)
		{
			var psiSourceFile = document.GetPsiSourceFile(Solution);
			if (psiSourceFile == null) return;
			if (!psiSourceFile.LanguageType.Is<T4ProjectFileType>()) return;
			var documentModel = Host.TryGetDocument(rdDocumentId)?.GetProtocolSynchronizerSafe()?.DocumentModel;
			if (documentModel == null)
			{
				Logger.Error("Editable entity not found in a document!");
				return;
			}

			var t4EditableEntityModel = documentModel.GetT4RdDocumentModel();
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
