using System.Linq;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	public sealed class T4TreeToDocumentTranslator : T4RangeTranslatorBase
	{
		public T4TreeToDocumentTranslator([NotNull] IT4FileLikeNode fileLikeNode) : base(fileLikeNode)
		{
		}

		public DocumentRange Translate(TreeTextRange range)
		{
			if (!range.IsValid() || !SourceFile.IsValid()) return DocumentRange.InvalidRange;
			// The start offset has to be situated as deep as possible
			// because there's no way for a highlighting to start at the root level but
			var atStart = FindIncludeAtOffset(range.StartOffset, false);
			var atEnd = FindIncludeAtOffset(range.EndOffset, atStart.Root == null);

			var include = atStart.Root;
			if (include != atEnd.Root)
				// Two different parts are overlapping,
				// it is impossible to select a document
				return DocumentRange.InvalidRange;

			// Let the included file handle the request
			if (include != null) return include.DocumentRangeTranslator.Translate(range);
			// The range is in the current document, handle it
			int rootStartOffset = FileLikeNode.GetTreeStartOffset().Offset;
			var resultingTextRange = new TextRange(atStart.Offset - rootStartOffset, atEnd.Offset - rootStartOffset);
			var documentRange = new DocumentRange(SourceFile.Document, resultingTextRange);
			return documentRange;
		}

		private T4OffsetFromFile FindIncludeAtOffset(TreeOffset offset, bool preferRoot)
		{
			// No includes, tree and document are matching
			if (!Includes.Any()) return new T4OffsetFromFile(offset.Offset, null);
			int includesLength = 0;
			int count = Includes.Count();
			for (int i = 0; i < count; i++)
			{
				var include = Includes.ElementAt(i);
				var includeRange = include.GetTreeTextRange();
				// The offset is before the include, in the root file
				if (offset < includeRange.StartOffset)
					return new T4OffsetFromFile((offset - includesLength).Offset, null);

				// The offset is inside the include
				if (offset < includeRange.EndOffset)
				{
					// We're on an edge position: we can be just after the end of the root file,
					// or just at the beginning of an include; we make the choice using the preferRoot parameter
					if ((offset == includeRange.StartOffset) && preferRoot)
						return new T4OffsetFromFile((offset - includesLength).Offset, null);
					return new T4OffsetFromFile(offset - includeRange.StartOffset, include);
				}

				includesLength += includeRange.Length;
			}

			// The offset is after the include, in the root file
			return new T4OffsetFromFile((offset - includesLength).Offset, null);
		}
	}
}
