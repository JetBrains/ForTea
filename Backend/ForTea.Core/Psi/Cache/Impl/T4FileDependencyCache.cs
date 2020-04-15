using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Collections;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	/// <summary>
	/// This cache stores the T4 file include dependency graph and manages dependent file invalidation:
	/// whenever a file is marked as dirty and its include list changes,
	/// this cache marks all the former dependencies and all the new dependencies as dirty.
	/// </summary>
	[PsiComponent]
	public sealed class T4FileDependencyCache : T4PsiAwareCacheBase<T4IncludeData, T4FileDependencyData>,
		IT4FileDependencyGraph, IT4FileGraphNotifier
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private T4GraphSinkSearcher GraphSinkSearcher { get; }

		[NotNull]
		private T4IndirectIncludeTransitiveClosureSearcher TransitiveClosureSearcher { get; }

		[NotNull]
		private IT4PsiFileSelector PsiFileSelector { get; }

		[NotNull]
		private IT4IncludeResolver IncludeResolver { get; }

		public event Action<IEnumerable<IPsiSourceFile>> OnFilesIndirectlyAffected;

		private IDictionary<IPsiSourceFile, T4ReversedFileDependencyData> ReversedMap { get; set; }

		[NotNull]
		private IShellLocks Locks { get; }

		[CanBeNull]
		private T4FileDependencyData TryGetIncludes([NotNull] IPsiSourceFile file) => Map.TryGetValue(file);

		[CanBeNull]
		private T4ReversedFileDependencyData TryGetIncluders([NotNull] IPsiSourceFile file) =>
			ReversedMap.TryGetValue(file);

		public T4FileDependencyCache(
			Lifetime lifetime,
			[NotNull] IPersistentIndexManager persistentIndexManager,
			[NotNull] T4GraphSinkSearcher graphSinkSearcher,
			[NotNull] T4IndirectIncludeTransitiveClosureSearcher transitiveClosureSearcher,
			[NotNull] IT4PsiFileSelector psiFileSelector,
			[NotNull] IT4IncludeResolver includeResolver,
			[NotNull] IShellLocks locks,
			[NotNull] ILogger logger
		) : base(lifetime, persistentIndexManager, T4FileDependencyDataMarshaller.Instance)
		{
			GraphSinkSearcher = graphSinkSearcher;
			TransitiveClosureSearcher = transitiveClosureSearcher;
			PsiFileSelector = psiFileSelector;
			IncludeResolver = includeResolver;
			Locks = locks;
			Logger = logger;
		}

		public IPsiSourceFile FindBestRoot(IPsiSourceFile include) =>
			GraphSinkSearcher.FindClosestSink(TryGetIncluders, include);

		[NotNull, ItemNotNull]
		private IEnumerable<IPsiSourceFile> FindIndirectIncludesTransitiveClosure([NotNull] IPsiSourceFile file) =>
			TransitiveClosureSearcher.FindClosure(TryGetIncludes, TryGetIncluders, file);

		protected override T4IncludeData Build(IT4File file) => new T4IncludeData(file
			.BlocksEnumerable
			.OfType<IT4IncludeDirective>()
			.Select(directive => IncludeResolver.ResolvePath(directive.ResolvedPath))
			.Where(path => !path.IsEmpty)
			.Distinct()
			.ToList()
		);

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			var data = (T4IncludeData) builtPart.NotNull();
			var includes = data.Includes;
			var oldIncludes = FindIndirectIncludesTransitiveClosure(sourceFile);
			base.Merge(sourceFile, new T4FileDependencyData(includes));
			var newIncludes = FindIndirectIncludesTransitiveClosure(sourceFile);
			OnFilesIndirectlyAffected?.Invoke(oldIncludes.Union(newIncludes));
			UpdateIncluders(sourceFile, includes);
		}

		private void UpdateIncluders([NotNull] IPsiSourceFile sourceFile, [NotNull] IList<FileSystemPath> includes)
		{
			// ReversedMap.TryGetValue is not a concurrent operation,
			// and races would be fatal here,
			// so we need to be sure that this is only called from the main thread
			Locks.AssertMainThread();
			UpdateIncluders(ReversedMap, sourceFile, includes);
		}

		[NotNull]
		public override object Load([NotNull] IProgressIndicator progress, bool enablePersistence)
		{
			base.Load(progress, enablePersistence);
			// Map is loaded on class instantiation
			progress.CurrentItemText = "Loading T4 file include caches";
			var result = new Dictionary<IPsiSourceFile, T4ReversedFileDependencyData>();
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			foreach (var (sourceFile, data) in Map)
			{
				// This lock is intentionally taken per-file to avoid too long blocking. Just in case.
				using (ReadLockCookie.Create())
				{
					UpdateIncluders(result, sourceFile, data.Includes);
				}
			}
			stopWatch.Stop();
			Logger.Verbose("Loading T4 cache took {0} ms", stopWatch.ElapsedMilliseconds);
			return result;
		}

		public override void MergeLoaded([NotNull] object data)
		{
			Logger.Assert(ReversedMap == null, "Attempted to load cache twice");
			ReversedMap = (IDictionary<IPsiSourceFile, T4ReversedFileDependencyData>) data;
		}

		private void UpdateIncluders(
			[NotNull] IDictionary<IPsiSourceFile, T4ReversedFileDependencyData> map,
			[NotNull] IPsiSourceFile includer,
			[NotNull] IList<FileSystemPath> includes
		)
		{
			foreach (var include in includes)
			{
				var includedSourceFile = PsiFileSelector.FindMostSuitableFile(include, includer);
				if (includedSourceFile == null) continue;
				var existingData = map.TryGetValue(includedSourceFile);
				if (existingData == null)
				{
					var includers = new List<FileSystemPath> {includer.GetLocation()};
					map[includedSourceFile] = new T4ReversedFileDependencyData(includers);
				}
				else
				{
					existingData.Includers.Add(includer.GetLocation());
				}
			}
		}
	}
}
