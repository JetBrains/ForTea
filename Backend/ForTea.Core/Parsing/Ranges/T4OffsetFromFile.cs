using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	/// <summary>
	/// A tree offset relative to <see cref="Root"/>.
	/// <see cref="Root"/><code> == null</code> means that the offset
	/// is not expressible in terms of root
	/// </summary>
	internal struct T4OffsetFromFile
	{
		[CanBeNull]
		public IT4File Root { get; }

		public int Offset { get; }

		public T4OffsetFromFile(int offset, [CanBeNull] IT4File root)
		{
			Offset = offset;
			Root = root;
		}
	}
}
