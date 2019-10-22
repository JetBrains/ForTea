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

		public T4DocumentRangeTranslator([NotNull] IT4File file, [NotNull] IPsiSourceFile sourceFile)
		{
			DocumentToTreeTranslator = new T4DocumentToTreeTranslator(file, sourceFile);
			TreeToDocumentTranslator = new T4TreeToDocumentTranslator(file, sourceFile);
		}

		public DocumentRange Translate(TreeTextRange range) => TreeToDocumentTranslator.Translate(range);
		public DocumentRange[] GetIntersectedOriginalRanges(TreeTextRange range) => new[] {Translate(range)};
		public TreeTextRange Translate(DocumentRange range) => DocumentToTreeTranslator.Translate(range);
	}
}
