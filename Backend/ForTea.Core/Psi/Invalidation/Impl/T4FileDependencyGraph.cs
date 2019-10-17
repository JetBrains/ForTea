using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation.Impl
{
	/// <summary>
	/// Track dependencies between T4 files: which file was included in which.
	/// </summary>
	public sealed class T4FileDependencyGraph : IT4FileDependencyGraph
	{
		[NotNull]
		private ILogger Logger { get; } = JetBrains.Util.Logging.Logger.GetLogger<T4FileDependencyGraph>();

		public T4FileDependencyGraph()
		{
			DirectIncludeGraph = new OneToSetMap<FileSystemPath, FileSystemPath>();
			ReversedIncludeGraph = new OneToSetMap<FileSystemPath, FileSystemPath>();
			SinkSearcher = new T4GraphSinkSearcher(ReversedIncludeGraph);
			IndirectIncludeSearcher =
				new T4IndirectIncludeTransitiveClosureSearcher(DirectIncludeGraph, ReversedIncludeGraph);
		}

		/// <summary>
		/// Also known as IncluderToIncludees
		/// </summary>
		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> DirectIncludeGraph { get; }

		/// <summary>
		/// Also known as IncludeeToIncluders
		/// </summary>
		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> ReversedIncludeGraph { get; }

		// This class is not stateless, but all of it's state is stored in the graph and this updated correctly
		[NotNull]
		private T4GraphSinkSearcher SinkSearcher { get; }

		// Same note as SinkSearcher
		[NotNull]
		private T4IndirectIncludeTransitiveClosureSearcher IndirectIncludeSearcher { get; }

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

		public IProjectFile FindBestRoot(IProjectFile file)
		{
			var rootPath = FindBestRoot(file.Location);
			var root = file
				.GetSolution()
				.FindProjectItemsByLocation(rootPath)
				.OfType<IProjectFile>()
				.SingleOrDefault();
			if (root == null)
			{
				Logger.Warn("Could not determine best root for a file");
				return file;
			}

			return root;
		}

		[NotNull]
		private FileSystemPath FindBestRoot([NotNull] FileSystemPath includee) => SinkSearcher.FindClosestSink(includee);

		public IEnumerable<FileSystemPath> FindIndirectIncludesTransitiveClosure(FileSystemPath path) =>
			IndirectIncludeSearcher.FindClosure(path);
	}
}
