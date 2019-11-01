using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	public sealed class T4DocumentRangeTranslator : IDocumentRangeTranslator
	{
		[NotNull]
		private T4DocumentToTreeTranslator DocumentToTreeTranslator { get; }

		[NotNull]
		private T4TreeToDocumentTranslator TreeToDocumentTranslator { get; }

		public T4DocumentRangeTranslator([NotNull] IT4FileLikeNode file)
		{
			DocumentToTreeTranslator = new T4DocumentToTreeTranslator(file);
			TreeToDocumentTranslator = new T4TreeToDocumentTranslator(file);
		}

		public DocumentRange Translate(TreeTextRange range) => TreeToDocumentTranslator.Translate(range);
		public DocumentRange[] GetIntersectedOriginalRanges(TreeTextRange range) => new[] {Translate(range)};
		public TreeTextRange Translate(DocumentRange range) => DocumentToTreeTranslator.Translate(range);
	}
}
