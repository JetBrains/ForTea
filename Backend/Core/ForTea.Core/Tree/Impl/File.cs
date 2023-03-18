using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
  // The manual implementation of some methods that cannot be implemented by PSI generator
  internal partial class File
  {
    public override PsiLanguageType Language => T4Language.Instance;
    public IPsiSourceFile LogicalPsiSourceFile { get; internal set; }
    public IPsiSourceFile PhysicalPsiSourceFile => GetSourceFile();
    public IEnumerable<IT4IncludedFile> Includes => this.Children<IT4IncludedFile>();
  }
}