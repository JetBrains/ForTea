using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation.Impl
{
	public sealed class T4IndirectIncludeTransitiveClosureSearcher
	{
		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> DirectIncludeGraph { get; }

		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> ReversedIncludeGraph { get; }

		public T4IndirectIncludeTransitiveClosureSearcher(
			[NotNull] OneToSetMap<FileSystemPath, FileSystemPath> directIncludeGraph,
			[NotNull] OneToSetMap<FileSystemPath, FileSystemPath> reversedIncludeGraph
		)
		{
			DirectIncludeGraph = directIncludeGraph;
			ReversedIncludeGraph = reversedIncludeGraph;
		}

		[NotNull, ItemNotNull]
		public IEnumerable<FileSystemPath> FindClosure([NotNull] FileSystemPath path) =>
			FindAllIncludees(FindAllIncluders(path));

		/// <summary>
		/// Performs DFS to collect all the files that include the current one,
		/// avoiding loops in includes if necessary
		/// </summary>
		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> FindAllIncluders([NotNull] FileSystemPath path)
		{
			var result = new JetHashSet<FileSystemPath>();
			FindAllChildren(path, ReversedIncludeGraph, result);
			return result;
		}

		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> FindAllIncludees([NotNull, ItemNotNull] IEnumerable<FileSystemPath> includers)
		{
			var result = new JetHashSet<FileSystemPath>();
			foreach (var includer in includers)
			{
				FindAllChildren(includer, DirectIncludeGraph, result);
			}

			return result;
		}

		private static void FindAllChildren(
			[NotNull] FileSystemPath path,
			[NotNull] OneToSetMap<FileSystemPath, FileSystemPath> graph,
			[NotNull, ItemNotNull] ISet<FileSystemPath> destination
		)
		{
			if (destination.Contains(path)) return;
			destination.Add(path);
			foreach (var child in graph[path])
			{
				FindAllChildren(child, graph, destination);
			}
		}
	}
}
