using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
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
		private T4GraphSinkSearcher GraphSinkSearcher { get; }

		[NotNull]
		private T4IndirectIncludeTransitiveClosureSearcher TransitiveClosureSearcher { get; }

		[NotNull]
		private IT4PsiFileSelector PsiFileSelector { get; }

		[NotNull]
		private IT4IncludeResolver IncludeResolver { get; }

		public event Action<IEnumerable<IPsiSourceFile>> OnFilesIndirectlyAffected;

		[CanBeNull]
		private T4FileDependencyData TryGetIncludes([NotNull] IPsiSourceFile file) => Map.TryGetValue(file);

		public T4FileDependencyCache(
			Lifetime lifetime,
			[NotNull] IPersistentIndexManager persistentIndexManager,
			[NotNull] T4GraphSinkSearcher graphSinkSearcher,
			[NotNull] T4IndirectIncludeTransitiveClosureSearcher transitiveClosureSearcher,
			[NotNull] IT4PsiFileSelector psiFileSelector,
			[NotNull] IT4IncludeResolver includeResolver
		) : base(lifetime, persistentIndexManager, T4FileDependencyDataMarshaller.Instance)
		{
			GraphSinkSearcher = graphSinkSearcher;
			TransitiveClosureSearcher = transitiveClosureSearcher;
			PsiFileSelector = psiFileSelector;
			IncludeResolver = includeResolver;
		}

		public IPsiSourceFile FindBestRoot(IPsiSourceFile include) =>
			GraphSinkSearcher.FindClosestSink(TryGetIncludes, include);

		[NotNull, ItemNotNull]
		private IEnumerable<IPsiSourceFile> FindIndirectIncludesTransitiveClosure([NotNull] IPsiSourceFile file) =>
			TransitiveClosureSearcher.FindClosure(TryGetIncludes, file);

		protected override T4IncludeData Build(IT4File file) => new T4IncludeData(file
			.BlocksEnumerable
			.OfType<IT4IncludeDirective>()
			.Select(directive => IncludeResolver.ResolvePath(directive.Path))
			.Where(path => !path.IsEmpty)
			.Distinct()
			.ToList()
		);

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			var data = (T4IncludeData) builtPart.NotNull();
			var includes = data.Includes;
			var includers = Map.TryGetValue(sourceFile)?.Includers ?? new List<FileSystemPath>();
			var oldIncludes = FindIndirectIncludesTransitiveClosure(sourceFile);
			base.Merge(sourceFile, new T4FileDependencyData(includes, includers));
			var newIncludes = FindIndirectIncludesTransitiveClosure(sourceFile);
			OnFilesIndirectlyAffected?.Invoke(oldIncludes.Union(newIncludes));
			UpdateIncluders(sourceFile, includes);
		}

		private void UpdateIncluders([NotNull] IPsiSourceFile sourceFile, [NotNull] IList<FileSystemPath> includes)
		{
			foreach (var include in includes)
			{
				var includedSourceFile = PsiFileSelector.FindMostSuitableFile(include, sourceFile);
				var existingData = Map.TryGetValue(includedSourceFile);
				if (existingData == null)
				{
					var includers = new List<FileSystemPath> {sourceFile.GetLocation()};
					Map[includedSourceFile] = new T4FileDependencyData(EmptyList<FileSystemPath>.Instance, includers);
				}
				else
				{
					var includers = existingData.Includers;
					includers.Add(sourceFile.GetLocation());
					Map[includedSourceFile] = new T4FileDependencyData(existingData.Includes, includers);
				}
			}
		}
	}
}
