using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl
{
	[SolutionComponent]
	public sealed class T4OutputExtensionFrontendNotifier : T4IndirectFileChangeObserverBase
	{
		[NotNull]
		private DocumentManager DocumentManager { get; }

		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		[NotNull]
		private IPsiFiles PsiFiles { get; }

		public T4OutputExtensionFrontendNotifier(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] DocumentManager documentManager,
			[NotNull] IT4FileDependencyGraph graph,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state,
			[NotNull] IPsiFiles psiFiles
		) : base(lifetime, notifier, services, state)
		{
			DocumentManager = documentManager;
			Graph = graph;
			PsiFiles = psiFiles;
		}

		protected override string ActivityName => "T4 frontend notification about file output extension";

		protected override void AfterCommitSync(ISet<IPsiSourceFile> indirectDependencies)
		{
			using var outerCookie = ReadLockCookie.Create();
			// Cache them in a closure, because they are lost otherwise
			PsiFiles.ExecuteAfterCommitAllDocuments(() =>
			{
				using var cookie = ReadLockCookie.Create();
				foreach (var file in indirectDependencies)
				{
					NotifyFrontend(file);
				}
			});
		}

		protected override void OnFilesIndirectlyAffected(
			T4FileInvalidationData data,
			ISet<IPsiSourceFile> indirectDependencies
		)
		{
			base.OnFilesIndirectlyAffected(data, indirectDependencies);
			indirectDependencies.Add(data.DirectlyAffectedFile);
		}

		public void NotifyFrontend([NotNull] IPsiSourceFile file)
		{
			var listener = TryGetListener(file);
			if (listener == null) return;
			string extension = GetTargetExtension(file);
			listener.ExtensionChanged(extension);
		}

		[CanBeNull]
		private string GetTargetExtension([NotNull] IPsiSourceFile file)
		{
			var root = Graph.FindBestRoot(file);
			var psi = root.GetPrimaryPsiFile();
			if (!(psi is IT4File t4File)) return null;
			string extension = t4File.GetTargetExtension();
			if (extension != null) return extension;
			var provider = file.GetSolution().GetComponent<IT4TemplateKindProvider>();
			if (provider.IsPreprocessedTemplate(root)) return null;
			return T4CSharpCodeGenerationUtils.DefaultTargetExtension;
		}

		[CanBeNull]
		private T4OutputExtensionChangeListener TryGetListener([NotNull] IPsiSourceFile file)
		{
			var projectFile = file.ToProjectFile();
			if (projectFile == null) return null;
			if (!projectFile.IsValid()) return null;
			var document = DocumentManager.TryGetDocument(projectFile);
			return document?.GetOutputExtensionChangeListener();
		}
	}
}
