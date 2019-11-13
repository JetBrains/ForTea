using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	public sealed class T4FileDependencyData
	{
		[NotNull, ItemNotNull]
		public List<FileSystemPath> Paths { get; }

		public T4FileDependencyData([NotNull, ItemNotNull] List<FileSystemPath> includes) => Paths = includes;
	}
}
