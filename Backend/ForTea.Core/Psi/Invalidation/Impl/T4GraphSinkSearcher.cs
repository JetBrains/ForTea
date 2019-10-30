using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Utils.Impl;
using GammaJul.ForTea.Core.Psi.Utils.Impl;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation.Impl
{
	public sealed class T4GraphSinkSearcher
	{
		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> Graph { get; }

		public T4GraphSinkSearcher([NotNull] OneToSetMap<FileSystemPath, FileSystemPath> graph) => Graph = graph;

		/// <summary>
		/// Perform a breadth-first search for a sink.
		/// Since the graph is not guaranteed to contain no cycles,
		/// it also checks that there are no includes.
		/// If there are no potential sinks
		/// (if, for example, the source is a sink itself,
		/// or there are only loops in hierarchy),
		/// returns source.
		/// </summary>
		[NotNull]
		public FileSystemPath FindClosestSink([NotNull] FileSystemPath source)
		{
			var guard = new T4BasicIncludeGuard();
			guard.StartProcessing(source);
			ISet<FileSystemPath> previousLayer;
			ISet<FileSystemPath> currentLayer = new JetHashSet<FileSystemPath>(new[] {source});
			do
			{
				previousLayer = currentLayer;
				currentLayer = previousLayer.SelectMany(it => Graph[it]).Where(path =>
				{
					bool canProcess = guard.CanProcess(path);
					if (canProcess) guard.StartProcessing(path);
					return canProcess;
				}).AsSet();

				var currentLayerSink = TrySelectSink(currentLayer);
				if (currentLayerSink != null) return currentLayerSink;
			}
			while (!currentLayer.IsEmpty());

			// So, there must be a recursion in includes.
			// This is not going to compile anyway,
			// so no support for this case
			return source;
		}

		[CanBeNull]
		private FileSystemPath TrySelectSink([NotNull] ISet<FileSystemPath> candidates) =>
			candidates.Where(IsSink).OrderBy(path => path.Name).FirstOrDefault();

		private bool IsSink([NotNull] FileSystemPath vertex) => Graph[vertex].IsEmpty();
	}
}
