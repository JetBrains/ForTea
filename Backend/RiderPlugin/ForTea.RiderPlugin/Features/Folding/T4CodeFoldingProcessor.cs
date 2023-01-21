using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
  public sealed class T4CodeFoldingProcessor : TreeNodeVisitor<FoldingHighlightingConsumer>, ICodeFoldingProcessor
  {
    private DocumentOffset? DirectiveFoldingStart { get; set; }
    private DocumentOffset? DirectiveFoldingEnd { get; set; }

    /// The directives we are interested in
    /// might reside very deep in the include tree,
    /// so we have to traverse more than just the top layer
    public bool InteriorShouldBeProcessed(ITreeNode element, FoldingHighlightingConsumer context) =>
      element is IT4IncludedFile;

    public bool IsProcessingFinished(FoldingHighlightingConsumer context) => false;

    public void ProcessBeforeInterior(ITreeNode element, FoldingHighlightingConsumer context)
    {
      if (!(element is IT4TreeNode t4Element)) return;
      if (!t4Element.IsVisibleInDocument()) return;
      t4Element.Accept(this, context);
    }

    public override void VisitDirectiveNode(IT4Directive directiveParam, FoldingHighlightingConsumer context)
    {
      // It is necessary to determine offset like this
      // because using a <see cref="TreeNodeExtensions.GetDocumentStartOffset(ITreeNode)"/>
      // would yield incorrect results in some edge cases
      DirectiveFoldingStart ??= directiveParam.GetDocumentRange().StartOffset;
      DirectiveFoldingEnd = directiveParam.GetDocumentRange().EndOffset;
    }

    public override void VisitNode(ITreeNode node, FoldingHighlightingConsumer context)
    {
      if (!(node is IT4TreeNode)) return;
      // Since this is a function that does not specify
      // what exactly we visited,
      // we must be visiting a token,
      // i.e. either newline or raw text
      if (node.NodeType == T4TokenNodeTypes.NEW_LINE) return;
      ProduceDirectiveFolding(context);
    }

    private void ProduceDirectiveFolding([NotNull] FoldingHighlightingConsumer context)
    {
      if (DirectiveFoldingStart == null || DirectiveFoldingEnd == null) return;
      var range = new DocumentRange(DirectiveFoldingStart.Value, DirectiveFoldingEnd.Value);
      context.AddDefaultPriorityFolding(T4CodeFoldingAttributes.Directive, range, "<#@ ... #>");
      DirectiveFoldingStart = null;
      DirectiveFoldingEnd = null;
    }

    public void ProcessAfterInterior(ITreeNode element, FoldingHighlightingConsumer context)
    {
      if (element.NextSibling != null) return;
      if (!(element is IT4TreeNode t4Element)) return;
      if (!t4Element.IsVisibleInDocument()) return;
      if (DirectiveFoldingStart == null || DirectiveFoldingEnd == null) return;
      var range = new DocumentRange(DirectiveFoldingStart.Value, DirectiveFoldingEnd.Value);
      context.AddDefaultPriorityFolding(T4CodeFoldingAttributes.Directive, range, "<#@ ... #>");
      DirectiveFoldingStart = null;
      DirectiveFoldingEnd = null;
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

    private static void AddFolding(
      [NotNull] FoldingHighlightingConsumer context,
      [NotNull] string id,
      [NotNull] IT4TreeNode node,
      [NotNull] string replacement
    )
    {
      if (!node.IsValid() || !node.IsVisibleInDocument()) return;
      context.AddDefaultPriorityFolding(id, node.GetDocumentRange(), replacement);
    }
  }
}