using System.Linq;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	public sealed class T4DocumentToTreeTranslator : T4RangeTranslatorBase
	{
		public T4DocumentToTreeTranslator([NotNull] IT4FileLikeNode fileLikeNode) : base(fileLikeNode)
		{
		}

		public TreeTextRange Translate(DocumentRange documentRange)
		{
			if (!documentRange.IsValid()) return TreeTextRange.InvalidRange;
			if (!SourceFile.IsValid()) return TreeTextRange.InvalidRange;
			if (!FileLikeNode.IsValid()) return TreeTextRange.InvalidRange;

			if (documentRange.Document != SourceFile.Document)
			{
				// That document might appear among the includes
				var rangeFromIncludes = Includes
					.Select(include => include.DocumentRangeTranslator.Translate(documentRange))
					.Where(textRange => textRange.IsValid())
					// Allow FirstOrDefault to return null
					.Select<TreeTextRange, TreeTextRange?>(it => it)
					.FirstOrDefault();
				return rangeFromIncludes ?? TreeTextRange.InvalidRange;
			}

			// The range is in the same document as the source file we are responsible for,
			// so we have no choice but to handle the request ourselves
			(int documentStartOffset, int documentEndOffset) = documentRange.TextRange;
			var rootStartOffset = FileLikeNode.GetTreeStartOffset();

			// No includes, tree and document are matching
			if (!Includes.Any())
				return new TreeTextRange(rootStartOffset + documentStartOffset, rootStartOffset + documentEndOffset);

			var treeStartOffset = Translate(rootStartOffset + documentStartOffset);
			if (!treeStartOffset.IsValid()) return TreeTextRange.InvalidRange;
			return TreeTextRange.FromLength(treeStartOffset, documentRange.Length);
		}

		private TreeOffset Translate(TreeOffset offsetInCurrentDocument)
		{
			int offset = 0;
			foreach (var include in Includes)
			{
				var includeRange = include.GetTreeTextRange();
				var finalOffset = offsetInCurrentDocument + offset;
				// The matching file offset starts before the current include, we got it
				if (finalOffset < includeRange.StartOffset) return finalOffset;
				offset += includeRange.Length;
			}

			// The offset is in the file, after the last include
			return offsetInCurrentDocument + offset;
		}
	}
}
