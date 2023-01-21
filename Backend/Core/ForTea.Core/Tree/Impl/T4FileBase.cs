using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
  /// <summary>
  /// The base class that contains mostly boilerplate code.
  /// I'll try to find a way to remove it
  /// </summary>
  public abstract class T4FileBase : FileElementBase
  {
    public abstract void Accept(TreeNodeVisitor visitor);
    public abstract void Accept<TContext>(TreeNodeVisitor<TContext> visitor, TContext context);
    public abstract TReturn Accept<TContext, TReturn>(TreeNodeVisitor<TContext, TReturn> visitor, TContext context);
  }
}