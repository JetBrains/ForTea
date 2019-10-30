using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	public struct T4FileSector
	{
		public T4FileSector(TreeTextRange range, [CanBeNull] IT4FileLikeNode include, int precedingIncludeLength)
		{
			Range = range;
			Include = include;
			PrecedingIncludeLength = precedingIncludeLength;
		}

		public int PrecedingIncludeLength { get; }
		public TreeTextRange Range { get; }
		[CanBeNull]
		public IT4FileLikeNode Include { get; }
		public bool IsValid() => Range.IsValid() && PrecedingIncludeLength >= 0;
		public void AssertValid() => Assertion.Assert(IsValid(), "IsValid()");
	}
}
