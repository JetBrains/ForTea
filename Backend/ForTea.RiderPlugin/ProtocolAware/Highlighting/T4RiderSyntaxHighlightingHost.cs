using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Documents;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting
{
	[SolutionComponent]
	public sealed class T4RiderSyntaxHighlightingHost
	{
		private Lifetime Lifetime { get; }

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
			Lifetime = lifetime;
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

			InitializeExtension(editableEntityLifetime, document);
		}

		private void InitializeExtension(Lifetime lifetime, [NotNull] IDocument targetDocument)
		{
			var file = targetDocument.GetPsiSourceFile(Solution);
			if (file == null) return;
			if (State.IsInitialUpdateFinished.Value)
			{
				Files.ExecuteAfterCommitAllDocuments(() => Notifier.NotifyFrontend(file));
				return;
			}

			var initialUpdateLifetime = lifetime.CreateNested();
			initialUpdateLifetime.AllowTerminationUnderExecution = true;
			State.IsInitialUpdateFinished.Change.Advise(initialUpdateLifetime.Lifetime, current =>
			{
				if (!current.HasNew || !current.New) return;
				using var cookie = ReadLockCookie.Create();
				Notifier.NotifyFrontend(file);
				// The lifetime of the component is used here to avoid memory leaks.
				// If we used adapterLifetime here, and the adapter dies
				// at the exact same moment as the initial update finishes,
				// the initialUpdateLifetime would never be terminated.
				// Also, cannot do that synchronously due to Lifetime API restrictions
				Solution.Locks.Queue(
					Lifetime,
					"T4: unsubscribe from InitialUpdateFinished",
					() => initialUpdateLifetime.Terminate()
				);
			});
		}
	}
}
