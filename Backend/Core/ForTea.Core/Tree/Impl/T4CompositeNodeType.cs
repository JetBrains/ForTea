using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
  public abstract class T4CompositeNodeType : CompositeNodeType
  {
    protected T4CompositeNodeType(string s, int index, Type nodeType) : base(s, index, nodeType)
    {
    }
  }
}