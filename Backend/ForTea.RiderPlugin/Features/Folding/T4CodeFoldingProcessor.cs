using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
	public sealed class T4CodeFoldingProcessor : TreeNodeVisitor<FoldingHighlightingConsumer>, ICodeFoldingProcessor
	{
		private int? DirectiveFoldingStart { get; set; }
		private int? DirectiveFoldingEnd { get; set; }

		// We are only interested in top-level structures, such as blocks
		public bool InteriorShouldBeProcessed(ITreeNode element, FoldingHighlightingConsumer context) => false;
		public bool IsProcessingFinished(FoldingHighlightingConsumer context) => false;

		public void ProcessBeforeInterior(ITreeNode element, FoldingHighlightingConsumer context)
		{
			if (!(element is IT4TreeNode t4Element)) return;
			t4Element.Accept(this, context);
		}

		public override void VisitDirectiveNode(IT4Directive directiveParam, FoldingHighlightingConsumer context)
		{
			DirectiveFoldingStart = DirectiveFoldingStart ?? directiveParam.GetTreeStartOffset().Offset;
			DirectiveFoldingEnd = directiveParam.GetTreeEndOffset().Offset;
		}

		public override void VisitNode(ITreeNode node, FoldingHighlightingConsumer context)
		{
			// Must be token
			if (node.NodeType == T4TokenNodeTypes.NEW_LINE) return;
			ProduceDirectiveFolding(node, context);
		}

		private void ProduceDirectiveFolding([NotNull] ITreeNode node, [NotNull] FoldingHighlightingConsumer context) =>
			ProduceDirectiveFolding(node.GetSourceFile().NotNull(), context);

		private void ProduceDirectiveFolding(
			[NotNull] IPsiSourceFile psiSourceFile,
			[NotNull] FoldingHighlightingConsumer context
		)
		{
			if (DirectiveFoldingStart == null || DirectiveFoldingEnd == null) return;
			var range = new DocumentRange(
				psiSourceFile.Document,
				new TextRange(DirectiveFoldingStart.Value, DirectiveFoldingEnd.Value));
			context.AddDefaultPriorityFolding(T4CodeFoldingAttributes.Directive, range, "<#@ ... #>");
			DirectiveFoldingStart = null;
			DirectiveFoldingEnd = null;
		}

		public void ProcessAfterInterior(ITreeNode element, FoldingHighlightingConsumer context)
		{
			if (element.NextSibling != null) return;
			if (DirectiveFoldingStart == null || DirectiveFoldingEnd == null) return;
			var range = new DocumentRange(
				element.GetSourceFile().NotNull().Document,
				new TextRange(DirectiveFoldingStart.Value, DirectiveFoldingEnd.Value));
			context.AddDefaultPriorityFolding(T4CodeFoldingAttributes.Directive, range, "<#@ ... #>");
		}

		public override void VisitExpressionBlockNode(
			IT4ExpressionBlock expressionBlockParam,
			[NotNull] FoldingHighlightingConsumer context
		) => AddFolding(context, T4CodeFoldingAttributes.ExpressionBlock, expressionBlockParam, "<#= ... #>");

		public override void VisitFeatureBlockNode(
			IT4FeatureBlock featureBlockParam,
			[NotNull] FoldingHighlightingConsumer context
		) => AddFolding(context, T4CodeFoldingAttributes.FeatureBlock, featureBlockParam, "<#+ ... #>");

		public override void VisitStatementBlockNode(
			IT4StatementBlock statementBlockParam,
			[NotNull] FoldingHighlightingConsumer context
		) => AddFolding(context, T4CodeFoldingAttributes.StatementBlock, statementBlockParam, "<# ... #>");

		private void AddFolding(
			[NotNull] FoldingHighlightingConsumer context,
			[NotNull] string id,
			[NotNull] ITreeNode node,
			[NotNull] string replacement
		) => context.AddDefaultPriorityFolding(id, node.GetDocumentRange(), replacement);
	}
}
