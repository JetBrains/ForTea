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

			int includedLength = 0;
			foreach (var include in Includes)
			{
				int includeEndOffset = include.GetTreeTextRange().EndOffset.Offset;
				if (includeEndOffset - includedLength > documentRange.TextRange.EndOffset)
					return BuildRange(documentRange, includedLength);

				if (includeEndOffset - includedLength == documentRange.TextRange.EndOffset)
				{
					if (documentRange.TextRange.StartOffset == documentRange.TextRange.EndOffset)
						// end of include
						return include.GetTreeTextRange();

					// range that start before the end of include statement
					return BuildRange(documentRange, includedLength);
				}

				if (includeEndOffset - includedLength > documentRange.TextRange.StartOffset)
					// document range crosses include boundary
					return TreeTextRange.InvalidRange;

				includedLength += include.GetTreeStartOffset().Offset - includeEndOffset;
			}

			return BuildRange(documentRange, includedLength);
		}

		private static TreeTextRange BuildRange(DocumentRange documentRange, int includedLength)
		{
			var start = new TreeOffset(documentRange.TextRange.StartOffset + includedLength);
			var end = new TreeOffset(documentRange.TextRange.EndOffset + includedLength);
			return new TreeTextRange(start, end);
		}
	}
}
