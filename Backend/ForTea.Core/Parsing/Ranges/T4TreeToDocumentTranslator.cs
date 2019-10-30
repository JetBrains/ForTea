using System.Collections.Generic;
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
		[NotNull]
		private IReadOnlyCollection<T4FileSector> Sectors { get; }

		public T4TreeToDocumentTranslator([NotNull] IT4FileLikeNode fileLikeNode) : base(fileLikeNode)
		{
			var sectors = ProduceSectors().AsList();
			ValidateSectors(sectors);
			Sectors = sectors;
		}

		[NotNull]
		private IEnumerable<T4FileSector> ProduceSectors()
		{
			var previousIncludeEnd = TreeOffset.Zero;
			int currentIncludeLength = 0;
			foreach (var include in Includes)
			{
				var includeRange = include.GetTreeTextRange();
				// This range is bound to be valid because includes cannot directly be followed by one another,
				// There has to be a gap for the 'include directive' between them
				var range = new TreeTextRange(previousIncludeEnd, includeRange.StartOffset);
				// The part of file that goes in between includes
				yield return new T4FileSector(range, FileLikeNode, currentIncludeLength);
				// The include itself
				yield return new T4FileSector(includeRange, include, currentIncludeLength);
				previousIncludeEnd = include.GetTreeEndOffset();
				currentIncludeLength += includeRange.Length;
			}

			// The remaining part of the file
			var fileEnd = FileLikeNode.GetTreeEndOffset();
			// The include directive might be the last directive in the file.
			// In that case, there would be no original file space left
			if (previousIncludeEnd == fileEnd) yield break;
			// Otherwise, this would be a valid range
			var lastRange = new TreeTextRange(previousIncludeEnd, fileEnd);
			yield return new T4FileSector(lastRange, FileLikeNode, currentIncludeLength);
		}

		private static void ValidateSectors([NotNull] IEnumerable<T4FileSector> sectors)
		{
			foreach (var sector in sectors)
			{
				sector.AssertValid();
			}
		}

		public DocumentRange Translate(TreeTextRange range)
		{
			if (!range.IsValid() || !SourceFile.IsValid()) return DocumentRange.InvalidRange;
			var sector = FindSectorAtRange(range);
			if (!sector.IsValid()) return DocumentRange.InvalidRange;

			// Let the included file handle the request
			if (sector.Include != null) return sector.Include.DocumentRangeTranslator.Translate(range);

			// The range is in the current document

			// The includes that appear before do not contribute to document offset
			int extraIncludeOffset = sector.PrecedingIncludeLength;

			// Neither do the other includes in the tree
			// (the ones that are included before the current file)
			int extraRootOffset = FileLikeNode.GetTreeStartOffset().Offset;

			var start = range.StartOffset - extraIncludeOffset - extraRootOffset;
			var end = range.EndOffset - extraIncludeOffset - extraRootOffset;
			var resultingTextRange = new TextRange(start.Offset, end.Offset);
			resultingTextRange.AssertValid();
			return new DocumentRange(SourceFile.Document, resultingTextRange);
		}

		private T4FileSector FindSectorAtRange(TreeTextRange range)
		{
			foreach (var sector in Sectors.Where(sector => sector.Range.Contains(range)))
			{
				return sector;
			}

			return new T4FileSector(TreeTextRange.InvalidRange, FileLikeNode, 0);
		}
	}
}
