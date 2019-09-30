using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation
{
	public interface IT4FileDependencyGraph
	{
		void UpdateIncludes([NotNull] FileSystemPath includer, [NotNull] ICollection<FileSystemPath> includees);

		[NotNull, ItemNotNull]
		IEnumerable<FileSystemPath> GetIncluders([NotNull] FileSystemPath includee);

		/// <summary>
		/// This is used for building correct PSI for .ttinclude files.
		/// T4 includes are similar to C++ ones, and symbols used in them
		/// can be defined in other .ttinclude files.
		/// It is only possible to track this by choosing the most complete context for each file,
		/// i.e. the uppermost file that includes the current one.
		/// </summary>
		[NotNull]
		FileSystemPath FindBestRoot([NotNull] FileSystemPath includee);
	}
}
