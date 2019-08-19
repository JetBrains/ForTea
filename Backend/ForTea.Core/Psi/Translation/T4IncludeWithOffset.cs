using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Translation
{
	public readonly ref struct T4IncludeWithOffset
	{
		[CanBeNull]
		public IT4Include Include { get; }

		public int Offset { get; }

		public T4IncludeWithOffset(int offset, [CanBeNull] IT4Include include = null)
		{
			Include = include;
			Offset = offset;
		}
	}
}
