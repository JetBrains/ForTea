using System.Collections.Generic;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
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
		private T4DirectiveInfoManager Manager { get; }

		[NotNull, ItemNotNull]
		private List<HighlightingInfo> MyHighlightings { get; } = new List<HighlightingInfo>();

		// Here I have the guarantee that new instance of this class is created for every analysis pass
		private bool SeenOutputDirective { get; set; }
		private bool SeenTemplateDirective { get; set; }

		[NotNull, ItemNotNull]
		public IReadOnlyList<HighlightingInfo> Highlightings => MyHighlightings;

		public T4IncludeAwareDaemonProcessVisitor([NotNull] T4DirectiveInfoManager manager) => Manager = manager;
		public bool InteriorShouldBeProcessed(ITreeNode element) => true;

		public void ProcessAfterInterior(ITreeNode element)
		{
		}

		public bool ProcessingIsFinished => false;

		public void ProcessBeforeInterior(ITreeNode element)
		{
			if (element is IT4Include include)
			{
				var destination = include.Path.ResolveT4File();
				if (destination != null) destination.ProcessDescendants(this);
				else ReportUnresolvedPath(include);
			}
			else if (element is IT4Directive directive)
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
		}

		private void ReportDuplicateDirective(IT4Directive directive) =>
			MyHighlightings.Add(new HighlightingInfo(directive.GetHighlightingRange(),
				new T4DuplicateDirectiveHighlighting(directive)));

		private void ReportUnresolvedPath([NotNull] IT4Include include)
		{
			var directive = include.PrevSibling as IT4Directive;
			Assertion.Assert(directive.IsSpecificDirective(Manager.Include), "unexpected include position");
			var attribute = directive.GetAttribute(Manager.Include.FileAttribute.Name);
			var value = attribute?.GetValueToken();
			if (value == null) return; // This case will be reported elsewhere?
			MyHighlightings.Add(new HighlightingInfo(directive.GetHighlightingRange(),
				new T4UnresolvedIncludeHighlighting(value)));
		}
	}
}
