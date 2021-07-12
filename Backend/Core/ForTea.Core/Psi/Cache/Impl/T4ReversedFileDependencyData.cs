using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	/// This class is isomorphic to <see cref="T4FileDependencyData"/>,
	/// but is kept separate to avoid confusing lists of includes with lists of includers
	public sealed class T4ReversedFileDependencyData
	{
		[NotNull, ItemNotNull]
		public IList<FileSystemPath> Includers { get; }

		public T4ReversedFileDependencyData([NotNull, ItemNotNull] IList<FileSystemPath> includes) => Includers = includes;
	}
}
