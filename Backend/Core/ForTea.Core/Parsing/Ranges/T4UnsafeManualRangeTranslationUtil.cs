using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
  public static class T4UnsafeManualRangeTranslationUtil
  {
    [Pure]
    public static DocumentOffset GetDocumentStartOffset([CanBeNull] ITreeNode element)
    {
      var file = element?.GetContainingFile();
      if (file == null) return DocumentOffset.InvalidOffset;
      return GetDocumentOffset(file, element.GetTreeStartOffset());
    }

    [Pure]
    private static DocumentOffset GetDocumentOffset([NotNull] IFile file, TreeOffset treeOffset)
    {
      if (file == null)
        throw new ArgumentNullException(nameof(file));

      return GetDocumentRange(file, new TreeTextRange(treeOffset)).StartOffset;
    }

    [Pure]
    private static DocumentRange GetDocumentRange([NotNull] IFile file, TreeTextRange range)
    {
      // The whole purpose of this class is to remove the following line:
      // Assertion.Assert(file.IsValid(), "file.IsValid()");

      if (!range.IsValid())
        return DocumentRange.InvalidRange;

      var tmpFile = (IFileImpl)file;
      var translator = tmpFile.SecondaryRangeTranslator;
      var tmpRange = range;
      while (translator != null)
      {
        tmpRange = translator.GeneratedToOriginal(tmpRange);
        if (!tmpRange.IsValid()) return DocumentRange.InvalidRange;

        tmpFile = (IFileImpl)translator.OriginalFile;
        if (tmpFile == null) return DocumentRange.InvalidRange;

        translator = tmpFile.SecondaryRangeTranslator;
      }

      var parsedDocumentRange = tmpFile.DocumentRangeTranslator.Translate(tmpRange);
      if (!parsedDocumentRange.IsValid())
        return DocumentRange.InvalidRange;

      return parsedDocumentRange;
    }
  }
}