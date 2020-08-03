using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl
{
	[SolutionComponent]
	public sealed class T4OutputExtensionFrontendNotifier
	{
		[NotNull]
		private DocumentManager DocumentManager { get; }

		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		public T4OutputExtensionFrontendNotifier(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] DocumentManager documentManager,
			[NotNull] IT4FileDependencyGraph graph
		)
		{
			DocumentManager = documentManager;
			Graph = graph;
			notifier.OnFilesIndirectlyAffected.Advise(lifetime, NotifyFrontend);
		}

		private void NotifyFrontend(T4FileInvalidationData data)
		{
			NotifyFrontend(data.DirectlyAffectedFile);
			foreach (var file in data.IndirectlyAffectedFiles)
			{
				NotifyFrontend(file);
			}
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
			var document = DocumentManager.TryGetDocument(projectFile);
			return document?.GetOutputExtensionChangeListener();
		}
	}
}
