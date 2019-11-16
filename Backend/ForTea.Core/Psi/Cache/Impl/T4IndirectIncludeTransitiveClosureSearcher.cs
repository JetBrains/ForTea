using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	public sealed class T4IndirectIncludeTransitiveClosureSearcher
	{
		[NotNull]
		private IDictionary<FileSystemPath, T4FileDependencyData> IncluderToIncludes { get; }

		[NotNull]
		private IDictionary<FileSystemPath, T4FileDependencyData> IncludeToIncluders { get; }

		public T4IndirectIncludeTransitiveClosureSearcher(
			[NotNull] IDictionary<FileSystemPath, T4FileDependencyData> includerToIncludes,
			[NotNull] IDictionary<FileSystemPath, T4FileDependencyData> includeToIncluders
		)
		{
			IncluderToIncludes = includerToIncludes;
			IncludeToIncluders = includeToIncluders;
		}

		[NotNull, ItemNotNull]
		public IEnumerable<FileSystemPath> FindClosure([NotNull] FileSystemPath path) =>
			FindAllIncludes(FindAllIncluders(path));

		/// <summary>
		/// Performs DFS to collect all the files that include the current one,
		/// avoiding loops in includes if necessary
		/// </summary>
		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> FindAllIncluders([NotNull] FileSystemPath path)
		{
			var result = new JetHashSet<FileSystemPath>();
			FindAllChildren(path, IncludeToIncluders, result);
			return result;
		}

		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> FindAllIncludes(
			[NotNull, ItemNotNull] IEnumerable<FileSystemPath> includers
		)
		{
			var result = new JetHashSet<FileSystemPath>();
			foreach (var includer in includers)
			{
				FindAllChildren(includer, IncluderToIncludes, result);
			}

			return result;
		}

		private static void FindAllChildren(
			[NotNull] FileSystemPath path,
			[NotNull] IDictionary<FileSystemPath, T4FileDependencyData> graph,
			[NotNull, ItemNotNull] ISet<FileSystemPath> destination
		)
		{
			if (destination.Contains(path)) return;
			destination.Add(path);
			var data = graph.TryGetValue(path);
			if (data == null) return;
			foreach (var child in data.Paths)
			{
				FindAllChildren(child, graph, destination);
			}
		}
	}
}
