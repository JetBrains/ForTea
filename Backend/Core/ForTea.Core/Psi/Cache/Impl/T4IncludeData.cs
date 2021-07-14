using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	public sealed class T4IncludeData
	{
		[NotNull, ItemNotNull]
		public IList<FileSystemPath> Includes { get; }

		public T4IncludeData([NotNull, ItemNotNull] IList<FileSystemPath> includes) => Includes = includes;
	}
}
