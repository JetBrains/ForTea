using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	// TODO: make it actually visitor
	public class T4IncludeAwareDaemonProcessVisitor : IRecursiveElementProcessor
	{
		[NotNull]
		private T4IncludeGuard Guard { get; }

		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull, ItemNotNull]
		private List<HighlightingInfo> MyHighlightings { get; } = new List<HighlightingInfo>();

		// Here I have the guarantee that new instance of this class is created for every analysis pass
		private bool SeenOutputDirective { get; set; }
		private bool SeenTemplateDirective { get; set; }
		private bool HasSeenRecursiveInclude { get; set; }

		[NotNull, ItemNotNull]
		public IReadOnlyList<HighlightingInfo> Highlightings => MyHighlightings;

		public T4IncludeAwareDaemonProcessVisitor(
			[NotNull] T4DirectiveInfoManager manager,
			[NotNull] IPsiSourceFile initialFile
		)
		{
			Manager = manager;
			HasSeenRecursiveInclude = false;
			Guard = new T4IncludeGuard();
			Guard.StartProcessing(initialFile);
		}

		public bool InteriorShouldBeProcessed(ITreeNode element) => true;

		public void ProcessAfterInterior(ITreeNode element)
		{
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
			if (directive.IsSpecificDirective(Manager.Output))
			{
				if (!SeenOutputDirective) SeenOutputDirective = true;
				else ReportDuplicateDirective(directive);
			}
			else if (directive.IsSpecificDirective(Manager.Template))
			{
				if (!SeenTemplateDirective) SeenTemplateDirective = true;
				else ReportDuplicateDirective(directive);
			}
		}

		private void ProcessInclude(IT4IncludeDirective include)
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

			var destination = include.Path.ResolveT4File(Guard);
			if (destination == null || sourceFile == null)
			{
				ReportUnresolvedPath(include);
				return;
			}

			Guard.StartProcessing(sourceFile);
			destination.ProcessDescendants(this);
			Guard.EndProcessing();
			if (HasSeenRecursiveInclude) ReportRecursiveInclude(include);
		}

		private void ReportDuplicateDirective(IT4Directive directive) =>
			MyHighlightings.Add(new HighlightingInfo(directive.GetHighlightingRange(),
				new T4DuplicateDirectiveHighlighting(directive)));

		private void AddHighlighting([NotNull] ITreeNode node, IHighlighting highlighting) =>
			MyHighlightings.Add(new HighlightingInfo(node.GetHighlightingRange(), highlighting));

		private void ReportUnresolvedPath([NotNull] IT4IncludeDirective include)
		{
			if (!Guard.IsOnTopLevel) return;
			var value = include.GetAttributes(Manager.Include.FileAttribute.Name)?.FirstOrDefault()?.Value;
			if (value == null) return;
			AddHighlighting(value, new T4UnresolvedIncludeHighlighting(value));
		}

		private void ReportRecursiveInclude([NotNull] IT4IncludeDirective include)
		{
			if (!Guard.IsOnTopLevel) return;
			var value = include.GetAttributes(Manager.Include.FileAttribute.Name)?.FirstOrDefault()?.Value;
			if (value == null) return;
			AddHighlighting(value, new T4RecursiveIncludeHighlighting(value));
		}

		private void ReportRedundantInclude([NotNull] IT4IncludeDirective include)
		{
			if (!Guard.IsOnTopLevel) return;
			var value = include.GetAttributes(Manager.Include.FileAttribute.Name)?.FirstOrDefault()?.Value;
			if (value == null) return;
			var directive = (value.Parent?.Parent as IT4Directive).NotNull();
			AddHighlighting(value, new T4RedundantIncludeHighlighting(directive));
		}
	}
}
