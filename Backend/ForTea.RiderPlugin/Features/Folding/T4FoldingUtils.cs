using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
	public static class T4FoldingUtils
	{
		/// <summary>
		/// Some nodes in T4 tree constitute include context.
		/// Here I'll call them invisible
		/// </summary>
		public static bool IsVisibleInDocument([NotNull] this IT4TreeNode node)
		{
			var file = (IT4File) node.GetContainingFile().NotNull();
			var nodeDocument = node.GetDocumentRange().Document;
			var visibleDocument = file.PhysicalPsiSourceFile?.Document;
			return nodeDocument == visibleDocument;
		}

		public static bool IsDirectlyInsideDirective([NotNull] this IT4TreeNode node)
		{
			static bool DirectiveIsCloser(ITreeNode node) => node switch
			{
				IT4Directive _ => true,
				IT4File _ => false,
				_ when node.Parent is IT4TreeNode parent => DirectiveIsCloser(parent),
				_ => false
			};

			return DirectiveIsCloser(node.Parent);
		}
	}
}
