using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
  public abstract class T4RangeTranslatorBase
  {
    /// <summary>
    /// The PSI file this translator is responsible for.
    /// It can be booth real T4 file and T4 file included into another one.
    /// </summary>
    [NotNull]
    protected IT4FileLikeNode FileLikeNode { get; }

    /// <summary>
    /// The source file from which the <see cref="FileLikeNode"/> is built.
    /// </summary>
    [NotNull]
    protected IPsiSourceFile SourceFile => FileLikeNode.LogicalPsiSourceFile;

    [NotNull, ItemNotNull] protected IEnumerable<IT4IncludedFile> Includes => FileLikeNode.Includes;

    protected T4RangeTranslatorBase([NotNull] IT4FileLikeNode fileLikeNode) => FileLikeNode = fileLikeNode;
  }
}