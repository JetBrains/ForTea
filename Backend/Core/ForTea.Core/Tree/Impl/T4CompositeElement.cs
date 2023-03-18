using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
  public abstract class T4CompositeElement : CompositeElement, IT4TreeNode
  {
    public override PsiLanguageType Language => T4Language.Instance;
    public abstract void Accept(TreeNodeVisitor visitor);
    public abstract void Accept<TContext>(TreeNodeVisitor<TContext> visitor, TContext context);
    public abstract TReturn Accept<TContext, TReturn>(TreeNodeVisitor<TContext, TReturn> visitor, TContext context);
  }
}