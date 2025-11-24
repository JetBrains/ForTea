using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Daemon.Syntax;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Syntax
{
  [Language(typeof(T4Language))]
  public class T4SyntaxHighlightingManager : SyntaxHighlightingManager
  {
    public override SyntaxHighlightingProcessor CreateProcessor(IPsiSourceFile sourceFile, IFile psiFile)
    {
      return new T4SyntaxHighlightingProcessor();
    }
  }
}