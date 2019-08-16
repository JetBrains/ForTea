using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Translation
{
	/// <summary>Translate T4 tree ranges from a file with includes to document ranges and vice-versa.</summary>
	public sealed class T4DocumentRangeTranslator : IDocumentRangeTranslator
	{
		[NotNull]
		private IT4IncludeOwner InitialFile { get; }

		[NotNull]
		private IPsiSourceFile SourceFile => InitialFile.GetSourceFile().NotNull();

		[NotNull, ItemNotNull]
		private IEnumerable<IT4Include> Includes => InitialFile.Children<IT4Include>();

		public T4DocumentRangeTranslator([NotNull] IT4File initialFile) => InitialFile = initialFile;

		public DocumentRange Translate(TreeTextRange range)
		{
			if (!range.IsValid() || !SourceFile.IsValid())
				return DocumentRange.InvalidRange;

			var includeAtStart = FindIncludeAtOffset(range.StartOffset, true);
			var includeAtEnd = FindIncludeAtOffset(range.EndOffset, includeAtStart.Include == null);

			// two different parts are overlapping
			if (includeAtStart.Include != includeAtEnd.Include)
				return DocumentRange.InvalidRange;

			// recursive includes
			if (includeAtStart.Include != null)
				return DocumentRange.InvalidRange;

			int rootStartOffset = InitialFile.GetTreeStartOffset().Offset;
			var resultRange = new TextRange(includeAtStart.Offset - rootStartOffset, includeAtEnd.Offset - rootStartOffset);
			return new DocumentRange(SourceFile.Document, resultRange);
		}

		public DocumentRange[] GetIntersectedOriginalRanges(TreeTextRange range) => new[] {Translate(range)};

		public TreeTextRange Translate(DocumentRange documentRange)
		{
			if (!documentRange.IsValid() || !SourceFile.IsValid())
				return TreeTextRange.InvalidRange;

			if (documentRange.Document != SourceFile.Document)
			{
				return TreeTextRange.InvalidRange;
			}

			var range = documentRange.TextRange;
			var rootStartOffset = InitialFile.GetTreeStartOffset();

			// no includes, tree and document are matching
			if (Includes.IsEmpty())
				return new TreeTextRange(rootStartOffset + range.StartOffset, rootStartOffset + range.EndOffset);

			var startOffset = Translate(range.StartOffset);
			if (!startOffset.IsValid())
				return TreeTextRange.InvalidRange;

			var endOffset = Translate(range.EndOffset);
			if (!endOffset.IsValid())
				return TreeTextRange.InvalidRange;

			return new TreeTextRange(startOffset, endOffset);
		}

		private T4IncludeWithOffset FindIncludeAtOffset(TreeOffset offset, bool preferRoot)
		{
			// no includes, tree and document are matching
			if (Includes.IsEmpty())
				return new T4IncludeWithOffset(offset.Offset);

			int includesLength = 0;
			var includes = Includes.AsList();
			foreach (var include in includes)
			{
				var includeRange = include.GetTreeTextRange();

				// the offset is before the include, in the root file
				if (offset < includeRange.StartOffset)
					return new T4IncludeWithOffset((offset - includesLength).Offset);

				// the offset is inside the include
				if (offset < includeRange.EndOffset)
				{
					// we're on an edge position: we can be just after the end of the root file,
					// or just at the beginning of an include; we make the choice using the preferRoot parameter
					if (offset == includeRange.StartOffset && preferRoot)
						return new T4IncludeWithOffset((offset - includesLength).Offset);
					return new T4IncludeWithOffset(offset - includeRange.StartOffset, include);
				}

				includesLength += includeRange.Length;
			}

			// the offset is after the include, in the root file
			return new T4IncludeWithOffset((offset - includesLength).Offset);
		}

		private TreeOffset Translate(int documentOffset)
		{
			int offset = 0;

			foreach (var include in Includes)
			{
				var includeRange = include.GetTreeTextRange();

				var finalOffset = new TreeOffset(documentOffset + offset);

				// the matching file offset starts before the include, we got it
				if (finalOffset < includeRange.StartOffset)
					return finalOffset;

				offset += includeRange.Length;
			}

			// the offset is in the file, after the last include
			return new TreeOffset(documentOffset + offset);
		}
	}
}
