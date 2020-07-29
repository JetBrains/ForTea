using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Collections.Viewable;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.Daemon;
using JetBrains.ReSharper.Host.Features.Documents;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting
{
	[SolutionComponent]
	public sealed class T4RiderRawTextHighlightingManager
	{
		public Lifetime Lifetime { get; }

		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private IPsiFiles PsiFiles { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		[NotNull]
		private DocumentHost Host { get; }

		[NotNull]
		private readonly Key<T4MarkupModelExtension> T4_MARKUP_MODEL_EXTENSION_KEY =
			new Key<T4MarkupModelExtension>("T4_MARKUP_MODEL_EXTENSION_KEY");

		public T4RiderRawTextHighlightingManager(
			Lifetime lifetime,
			[NotNull] ILogger logger,
			[NotNull] RiderMarkupHost markupHost,
			[NotNull] IPsiFiles psiFiles,
			[NotNull] ISolution solution,
			[NotNull] IT4FileDependencyGraph graph,
			[NotNull] DocumentHost host
		)
		{
			Lifetime = lifetime;
			Logger = logger;
			PsiFiles = psiFiles;
			Solution = solution;
			Graph = graph;
			Host = host;
			markupHost.MarkupAdapters.View(lifetime, CreateHandler);
		}

		private void CreateHandler(Lifetime adapterLifetime, [NotNull] IRiderMarkupModelAdapter adapter)
		{
			var t4MarkupExtension = adapter.GetExtension(T4_MARKUP_MODEL_EXTENSION_KEY);
			if (t4MarkupExtension == null)
			{
				Logger.Error("Markup model extension {0} wasn't found!", T4_MARKUP_MODEL_EXTENSION_KEY);
				return;
			}

			t4MarkupExtension.RawTextExtension.Set((lifetime, unit) =>
			{
				var task = new RdTask<string>();
				using var outerReadLockCookie = ReadLockCookie.Create(); // todo remove
				var targetDocument = adapter.DocumentMarkup.Document;
				var path = targetDocument.TryGetFilePath();
				Solution.GetProtocolSolution()
					.GetFileSystemModel()
					.RefreshPaths
					.Start(Lifetime, new RdRefreshRequest(new List<string> {path.FullPath}, false))
					.Result.Advise(Lifetime, _ =>
					{
						using var innerReadLockCookie = ReadLockCookie.Create();
						PsiFiles.ExecuteAfterCommitAllDocuments(() =>
						{
							var file = targetDocument.GetPsiSourceFile(Solution).NotNull();
							var root = Graph.FindBestRoot(file);
							if (root.GetPrimaryPsiFile() is IT4File t4Root)
							{
								string targetExtension = t4Root.GetTargetExtension();
								task.Set(targetExtension);
								return;
							}

							task.Set(null as string);
						});
					});
				return task;
			});
		}
	}
}
