using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
  public sealed class T4DocumentRangeTranslator : IDocumentRangeTranslator
  {
    [NotNull] private T4DocumentToTreeTranslator DocumentToTreeTranslator { get; }

    [NotNull] private T4TreeToDocumentTranslator TreeToDocumentTranslator { get; }

    public T4DocumentRangeTranslator([NotNull] IT4FileLikeNode file)
    {
      DocumentToTreeTranslator = new T4DocumentToTreeTranslator(file);
      TreeToDocumentTranslator = new T4TreeToDocumentTranslator(file);
    }

    public DocumentRange Translate(TreeTextRange range) => TreeToDocumentTranslator.Translate(range);
    public FrugalLocalList<DocumentRange> GetIntersectedOriginalRanges(TreeTextRange range)
      => FrugalLocalList<DocumentRange>.Of(Translate(range));
    
    public TreeTextRange Translate(DocumentRange range) => DocumentToTreeTranslator.Translate(range);
  }
}