using System.Linq;
using GammaJul.ForTea.Core.Daemon.Attributes;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	public sealed class T4SyntaxHighlightingVisitor : TreeNodeVisitor<IHighlightingConsumer>
	{
		public override void VisitMacroNode(IT4Macro macroParam, IHighlightingConsumer context)
		{
			((IT4TreeNode) macroParam.Dollar).Accept(this, context);
			((IT4TreeNode) macroParam.LeftParenthesis).Accept(this, context);
			((IT4TreeNode) macroParam.RightParenthesis).Accept(this, context);
			var value = macroParam.RawAttributeValue;
			if (value == null) return;
			var range = value.GetDocumentRange();
			context.AddHighlighting(new MacroHighlighting(range));
		}

		public override void VisitEnvironmentVariableNode(
			IT4EnvironmentVariable environmentVariableParam,
			IHighlightingConsumer context
		)
		{
			foreach (var node in environmentVariableParam.StartPercentEnumerable.Cast<IT4Token>())
			{
				node.Accept(this, context);
			}

			var value = environmentVariableParam.RawAttributeValue;
			if (value == null) return;
			var range = value.GetDocumentRange();
			context.AddHighlighting(new EnvironmentVariableHighlighting(range));
		}

		public override void VisitNode(ITreeNode node, [NotNull] IHighlightingConsumer context)
		{
			if (!IsAttributeValue(node)) return;
			string id = T4HighlightingAttributeIds.ATTRIBUTE_VALUE;
			var highlighting = new ReSharperSyntaxHighlighting(id, null, node.GetDocumentRange());
			context.AddHighlighting(highlighting);
		}

		private static bool IsAttributeValue([NotNull] ITreeNode node) =>
			node.GetParentOfType<IT4AttributeValue>() != null;

		public override void VisitDirectiveNode(
			IT4Directive directiveParam,
			[NotNull] IHighlightingConsumer context
		)
		{
			const string id = T4HighlightingAttributeIds.DIRECTIVE;
			var highlighting = new ReSharperSyntaxHighlighting(id, null, directiveParam.Name.GetDocumentRange());
			context.AddHighlighting(highlighting);
		}

		public override void VisitAttributeNameNode(
			IT4AttributeName attributeNameParam,
			[NotNull] IHighlightingConsumer context
		)
		{
			string id = T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE;
			var highlighting = new ReSharperSyntaxHighlighting(id, null, attributeNameParam.GetDocumentRange());
			context.AddHighlighting(highlighting);
		}
	}
}
