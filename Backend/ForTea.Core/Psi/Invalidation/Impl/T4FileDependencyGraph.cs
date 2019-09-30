using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation.Impl
{
	/// <summary>
	/// Track dependencies between T4 files: which file was included in which.
	/// </summary>
	public sealed class T4FileDependencyGraph : IT4FileDependencyGraph
	{
		/// <summary>
		/// Also known as IncluderToIncludees
		/// </summary>
		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> DirectIncludeGraph { get; } =
			new OneToSetMap<FileSystemPath, FileSystemPath>();

		/// <summary>
		/// Also known as IncludeeToIncluders
		/// </summary>
		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> ReversedIncludeGraph { get; } =
			new OneToSetMap<FileSystemPath, FileSystemPath>();

		public void UpdateIncludes(FileSystemPath includer, ICollection<FileSystemPath> includees)
		{
			foreach (var includee in DirectIncludeGraph[includer])
			{
				ReversedIncludeGraph.Remove(includee, includer);
			}

			DirectIncludeGraph.RemoveKey(includer);
			if (includees.Count <= 0) return;
			DirectIncludeGraph.AddRange(includer, includees);
			foreach (var includee in includees)
			{
				ReversedIncludeGraph.Add(includee, includer);
			}
		}

		public IEnumerable<FileSystemPath> GetIncluders(FileSystemPath includee) => ReversedIncludeGraph[includee];
		public FileSystemPath FindBestRoot(FileSystemPath includee) => throw new System.NotImplementedException();
	}
}
