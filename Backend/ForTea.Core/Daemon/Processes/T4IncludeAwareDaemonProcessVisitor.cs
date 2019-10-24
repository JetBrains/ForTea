using System.Collections.Generic;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Psi.Utils.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	// TODO: make it actually visitor
	public sealed class T4IncludeAwareDaemonProcessVisitor : IRecursiveElementProcessor
	{
		[NotNull]
		private IT4IncludeGuard<IPsiSourceFile> Guard { get; }

		[NotNull, ItemNotNull]
		private List<HighlightingInfo> MyHighlightings { get; } = new List<HighlightingInfo>();

		// Here I have the guarantee that new instance of this class is created for every analysis pass
		private bool SeenOutputDirective { get; set; }
		private bool SeenTemplateDirective { get; set; }
		private bool HasSeenRecursiveInclude { get; set; }

		[NotNull, ItemNotNull]
		public IReadOnlyList<HighlightingInfo> Highlightings => MyHighlightings;

		public T4IncludeAwareDaemonProcessVisitor([NotNull] IPsiSourceFile initialFile)
		{
			HasSeenRecursiveInclude = false;
			Guard = new T4ContextTrackingIncludeGuard();
			Guard.StartProcessing(initialFile);
		}

		public bool InteriorShouldBeProcessed(ITreeNode element) => true;

		public void ProcessAfterInterior(ITreeNode element)
		{
			if (!(element is IT4IncludeDirective include)) return;
			Guard.EndProcessing();
			if (HasSeenRecursiveInclude) ReportRecursiveInclude(include);
		}

		public bool ProcessingIsFinished => false;

		public void ProcessBeforeInterior(ITreeNode element)
		{
			switch (element)
			{
				case IT4IncludeDirective include:
					ProcessInclude(include);
					break;
				case IT4Directive directive:
					ProcessDirective(directive);
					break;
			}
		}

		private void ProcessDirective(IT4Directive directive)
		{
			switch (directive)
			{
				case IT4OutputDirective _ when !SeenOutputDirective:
					SeenOutputDirective = true;
					break;
				case IT4OutputDirective _:
					ReportDuplicateDirective(directive);
					break;
				case IT4TemplateDirective _ when !SeenTemplateDirective:
					SeenTemplateDirective = true;
					break;
				case IT4TemplateDirective _:
					ReportDuplicateDirective(directive);
					break;
			}
		}

		private void ProcessInclude([NotNull] IT4IncludeDirective include)
		{
			var sourceFile = include.Path.Resolve();
			if (sourceFile != null)
			{
				if (!Guard.CanProcess(sourceFile))
				{
					HasSeenRecursiveInclude = true;
					ReportRecursiveInclude(include);
					return;
				}

				if (include.Once && Guard.HasSeenFile(sourceFile))
				{
					ReportRedundantInclude(include);
					return;
				}
			}

			if (sourceFile == null)
			{
				ReportUnresolvedPath(include);
				return;
			}

			Guard.StartProcessing(sourceFile);
		}

		private void ReportDuplicateDirective([NotNull] IT4Directive directive)
		{
			var warning = new DuplicateDirectiveWarning(directive);
			var highlightingInfo = new HighlightingInfo(directive.GetHighlightingRange(), warning);
			MyHighlightings.Add(highlightingInfo);
		}

		private void AddHighlighting([NotNull] ITreeNode node, [NotNull] IHighlighting highlighting) =>
			MyHighlightings.Add(new HighlightingInfo(node.GetHighlightingRange(), highlighting));

		private void ReportUnresolvedPath([NotNull] IT4IncludeDirective include)
		{
			var name = include.Name;
			AddHighlighting(name, new UnresolvedIncludeWarning(name));
		}

		private void ReportRecursiveInclude([NotNull] IT4IncludeDirective include)
		{
			var value = include.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value;
			if (value == null) return;
			AddHighlighting(value, new RecursiveIncludeError(value));
		}

		private void ReportRedundantInclude([NotNull] IT4IncludeDirective include)
		{
			var value = include.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value;
			if (value == null) return;
			AddHighlighting(value, new RedundantIncludeWarning(include));
		}
	}
}
