using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Collections.Viewable;
using JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting
{
	[SolutionComponent]
	public sealed class T4RiderRawTextHighlightingManager
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
		private readonly Key<T4MarkupModelExtension> T4_MARKUP_MODEL_EXTENSION_KEY =
			new Key<T4MarkupModelExtension>("T4_MARKUP_MODEL_EXTENSION_KEY");

		public T4RiderRawTextHighlightingManager(
			Lifetime lifetime,
			[NotNull] ILogger logger,
			[NotNull] RiderMarkupHost markupHost,
			[NotNull] T4OutputExtensionFrontendNotifier notifier,
			[NotNull] ISolution solution,
			[NotNull] IPsiCachesState state
		)
		{
			Lifetime = lifetime;
			Logger = logger;
			Notifier = notifier;
			Solution = solution;
			State = state;
			markupHost.MarkupAdapters.View(lifetime, CreateHandler);
		}

		private void CreateHandler(Lifetime adapterLifetime, [NotNull] IRiderMarkupModelAdapter adapter)
		{
			var t4MarkupExtension = adapter.GetExtension(T4_MARKUP_MODEL_EXTENSION_KEY);
			if (t4MarkupExtension == null)
			{
				// TODO: debug wtf
				Logger.Error("Markup model extension {0} wasn't found!", T4_MARKUP_MODEL_EXTENSION_KEY);
				return;
			}

			var targetDocument = adapter.DocumentMarkup.Document;
			targetDocument.CreateListener(
				adapterLifetime,
				new T4OutputExtensionChangeListener(t4MarkupExtension.RawTextExtension)
			);

			var file = targetDocument.GetPsiSourceFile(Solution);
			if (file == null) return;
			if (State.IsInitialUpdateFinished.Value)
			{
				Notifier.NotifyFrontend(file);
			}
			else
			{
				var initialUpdateLifetime = adapterLifetime.CreateNested();
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
}
