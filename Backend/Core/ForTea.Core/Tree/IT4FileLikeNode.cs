using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Tree
{
  public interface IT4FileLikeNode : IT4TreeNode
  {
    [NotNull] IDocumentRangeTranslator DocumentRangeTranslator { get; }

    /// <summary>
    /// Primary PSI for a T4 file contains a lot of context,
    /// including the files that include the current T4 file
    /// and indirect includes.
    /// This is done in order to provide the most complete intelligent support.
    /// This means that PSI now doesn't just contain contents of the source file it corresponds to,
    /// but also a bunch of other files.
    /// <see cref="LogicalPsiSourceFile"/> is a reference to the file that was the source of tokens
    /// for the current <see cref="IT4FileLikeNode"/>.
    /// </summary>
    [NotNull]
    IPsiSourceFile LogicalPsiSourceFile { get; }

    /// <summary>
    /// Which source file caused the current tree of T4 files to be built.
    /// Note that within the same tree there can be multiple nodes with different
    /// <see cref="LogicalPsiSourceFile"/> but all of them share the same <see cref="PhysicalPsiSourceFile"/>
    /// </summary>
    [CanBeNull]
    IPsiSourceFile PhysicalPsiSourceFile { get; }

    [NotNull, ItemNotNull] IEnumerable<IT4IncludedFile> Includes { get; }

    [Pure]
    [CanBeNull]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [Obsolete("You should state explicitly which source file you are interested in", true)]
    new IPsiSourceFile GetSourceFile();

    TreeNodeCollection<IT4Block> Blocks { get; }
  }
}