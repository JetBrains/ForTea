using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Collections;
using JetBrains.DataFlow;
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

		public Signal<T4FileInvalidationData> OnFilesIndirectlyAffected { get; }

		private IDictionary<IPsiSourceFile, T4ReversedFileDependencyData> ReversedMap { get; set; }

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
			[NotNull] ILogger logger
		) : base(lifetime, persistentIndexManager, T4FileDependencyDataMarshaller.Instance)
		{
			GraphSinkSearcher = graphSinkSearcher;
			TransitiveClosureSearcher = transitiveClosureSearcher;
			PsiFileSelector = psiFileSelector;
			IncludeResolver = includeResolver;
			Logger = logger;
			OnFilesIndirectlyAffected = new Signal<T4FileInvalidationData>(
				lifetime,
				"T4FileDependencyCache notification about a change in indirect includes"
			);
		}

		public IPsiSourceFile FindBestRoot(IPsiSourceFile include) =>
			GraphSinkSearcher.FindClosestSink(TryGetIncluders, include);

		[NotNull, ItemNotNull]
		private JetHashSet<IPsiSourceFile> FindIndirectIncludesTransitiveClosure([NotNull] IPsiSourceFile file) =>
			TransitiveClosureSearcher.FindClosure(TryGetIncludes, TryGetIncluders, file);

		protected override T4IncludeData Build(IT4File file)
		{
			if (file.PhysicalPsiSourceFile?.IsBeingIndirectlyUpdated() == true)
			{
				// Since the contents of this file did not change,
				// the list of its direct includes did not change either,
				// so there's no point in doing anything here
				return null;
			}

			return new T4IncludeData(file
				.GetThisAndChildrenOfType<IT4IncludeDirective>()
				.Where(directive => directive.IsVisibleInDocument())
				.Select(directive => IncludeResolver.ResolvePath(directive.ResolvedPath))
				.Where(path => !path.IsEmpty)
				.Distinct()
				.ToList()
			);
		}

		[CanBeNull]
		private IEnumerable<IPsiSourceFile> GetIncludes([NotNull] IPsiSourceFile sourceFile) => Map
			.TryGetValue(sourceFile)
			?.Includes
			.Select(path => PsiFileSelector.FindMostSuitableFile(path, sourceFile));

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			if (builtPart == null) {
				// Indirect dependency invalidation
				RemoveFromDirty(sourceFile);
				return;
			}
			var data = (T4IncludeData) builtPart.NotNull();

			var oldTransitiveIncludes = FindIndirectIncludesTransitiveClosure(sourceFile);
			var oldIncludes = new JetHashSet<IPsiSourceFile>(
				GetIncludes(sourceFile) ?? EmptyList<IPsiSourceFile>.Instance
			);

			base.Merge(sourceFile, new T4FileDependencyData(data.Includes));

			var newTransitiveIncludes = FindIndirectIncludesTransitiveClosure(sourceFile);
			var newIncludes = new JetHashSet<IPsiSourceFile>(
				GetIncludes(sourceFile) ?? EmptyList<IPsiSourceFile>.Instance
			);

			var fileInvalidationData = new T4FileInvalidationData(
				oldTransitiveIncludes.Union(newTransitiveIncludes).Except(sourceFile),
				sourceFile
			);
			OnFilesIndirectlyAffected.Fire(fileInvalidationData);

			oldIncludes.Compare(newIncludes, out var addedItems, out var removedItems);
			UpdateIncluders(ReversedMap, sourceFile, addedItems, removedItems);
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
					var includes = new JetHashSet<IPsiSourceFile>(
						data.Includes.Select(include => PsiFileSelector.FindMostSuitableFile(include, sourceFile))
					);
					UpdateIncluders(result, sourceFile, includes, JetHashSet<IPsiSourceFile>.Empty);
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
			[NotNull, ItemNotNull] IEnumerable<IPsiSourceFile> addedIncludes,
			[NotNull, ItemNotNull] IEnumerable<IPsiSourceFile> removedIncludes
		)
		{
			foreach (var removedInclude in removedIncludes.WhereNotNull())
			{
				var existingData = map.TryGetValue(removedInclude);
				existingData?.Includers.Remove(includer.GetLocation());
				if (existingData?.Includers.Count == 0) map.Remove(removedInclude);
			}

			foreach (var addedInclude in addedIncludes.WhereNotNull())
			{
				var existingData = map.TryGetValue(addedInclude);
				if (existingData == null)
				{
					var includers = new List<FileSystemPath> {includer.GetLocation()};
					map[addedInclude] = new T4ReversedFileDependencyData(includers);
				}
				else
				{
					existingData.Includers.Add(includer.GetLocation());
				}
			}
		}
	}
}
