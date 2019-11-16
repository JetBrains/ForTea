using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Attributes;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Processes
{
	/// <summary>Process that highlights block tags and missing token errors.</summary>
	internal sealed class T4HighlightingProcess : IDaemonStageProcess, IRecursiveElementProcessor
	{
		[NotNull, ItemNotNull] private readonly List<HighlightingInfo> _highlightings = new List<HighlightingInfo>();
		public IDaemonProcess DaemonProcess { get; }

		/// <summary>Gets the associated T4 file.</summary>
		private IT4File File { get; }

		public bool InteriorShouldBeProcessed(ITreeNode element) => !(element is IT4File);

		public void ProcessAfterInterior(ITreeNode element)
		{
		}

		public bool ProcessingIsFinished => false;

		public void Execute(Action<DaemonStageResult> commiter)
		{
			File.ProcessDescendants(this);
			var solution = File.GetSolution();
			var relevantHighlightings = _highlightings
				.Where(info => info.Range.Document.GetPsiSourceFile(solution) == File.PhysicalPsiSourceFile);
			commiter(new DaemonStageResult(relevantHighlightings.ToArray()));
		}

		private void AddHighlighting(DocumentRange range, [NotNull] IHighlighting highlighting) =>
			_highlightings.Add(new HighlightingInfo(range, highlighting));

		public void ProcessBeforeInterior(ITreeNode element)
		{
			string attributeId = GetHighlightingAttributeId(element);
			if (attributeId != null)
			{
				DocumentRange range = element.GetHighlightingRange();
				AddHighlighting(range, new ReSharperSyntaxHighlighting(attributeId, string.Empty, range));
			}
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element)
		{
			if (!(element.GetTokenType() is T4TokenNodeType tokenType)) return null;
			if (tokenType.IsTag) return T4HighlightingAttributeIds.BLOCK_TAG;
			if (tokenType == T4TokenNodeTypes.QUOTE
			    || tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE
			    || tokenType == T4TokenNodeTypes.EQUAL
			    || tokenType == T4TokenNodeTypes.DOLLAR
			    || tokenType == T4TokenNodeTypes.PERCENT
			    || tokenType == T4TokenNodeTypes.LEFT_PARENTHESIS
			    || tokenType == T4TokenNodeTypes.RIGHT_PARENTHESIS)
				return T4HighlightingAttributeIds.ATTRIBUTE_VALUE;
			if (T4Lexer.DirectiveTypes[tokenType]) return T4HighlightingAttributeIds.DIRECTIVE;
			if (tokenType == T4TokenNodeTypes.TOKEN) return T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE;

			return null;
		}

		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
		{
			File = file;
			DaemonProcess = daemonProcess;
		}
	}
}
