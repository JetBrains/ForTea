using GammaJul.ForTea.Core.Daemon.Attributes;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	public sealed class T4SyntaxHighlightingVisitor : TreeNodeVisitor
	{
		[NotNull]
		private IHighlightingConsumer Context { get; }

		public T4SyntaxHighlightingVisitor([NotNull] IHighlightingConsumer context) => Context = context;

		public override void VisitMacroNode(IT4Macro macroParam)
		{
			((IT4TreeNode) macroParam.Dollar).Accept(this);
			((IT4TreeNode) macroParam.LeftParenthesis)?.Accept(this);
			((IT4TreeNode) macroParam.RightParenthesis)?.Accept(this);
			var value = macroParam.RawAttributeValue;
			if (value == null) return;
			var range = value.GetDocumentRange();
			Context.AddHighlighting(new MacroHighlighting(range));
		}

		public override void VisitEnvironmentVariableNode(IT4EnvironmentVariable environmentVariableParam)
		{
			(environmentVariableParam.StartPercent as IT4Token)?.Accept(this);
			(environmentVariableParam.EndPercent as IT4Token)?.Accept(this);

			var value = environmentVariableParam.RawAttributeValue;
			if (value == null) return;
			var range = value.GetDocumentRange();
			Context.AddHighlighting(new EnvironmentVariableHighlighting(range));
		}

		public override void VisitNode(ITreeNode node)
		{
			if (!IsAttributeValue(node)) return;
			AddHighlighting(T4HighlightingAttributeIds.ATTRIBUTE_VALUE, node);
		}

		public override void VisitCodeBlockNode(IT4CodeBlock codeBlockParam) =>
			AddHighlighting(T4HighlightingAttributeIds.CODE_BLOCK, codeBlockParam);

		private static bool IsAttributeValue([NotNull] ITreeNode node) =>
			node.GetParentOfType<IT4AttributeValue>() != null;

		public override void VisitDirectiveNode(IT4Directive directiveParam) =>
			AddHighlighting(T4HighlightingAttributeIds.DIRECTIVE, directiveParam.Name);

		public override void VisitAttributeNameNode(IT4AttributeName attributeNameParam) =>
			AddHighlighting(T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE, attributeNameParam);

		private void AddHighlighting([NotNull] string id, [CanBeNull] ITreeNode node)
		{
			if (node?.IsVisibleInDocument() != true) return;
			var highlighting = new ReSharperSyntaxHighlighting(id, null, node.GetDocumentRange());
			Context.AddHighlighting(highlighting);
		}
	}
}
