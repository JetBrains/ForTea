using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Tree
{
	/// <summary>Contract representing a T4 tree node.</summary>
	public interface IT4TreeNode : ITreeNode
	{
		void Accept(TreeNodeVisitor visitor);
		void Accept<TContext>(TreeNodeVisitor<TContext> visitor, TContext context);
		TReturn Accept<TContext, TReturn>(TreeNodeVisitor<TContext, TReturn> visitor, TContext context);
	}
}
