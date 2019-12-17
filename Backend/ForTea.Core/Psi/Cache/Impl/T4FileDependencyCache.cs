using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Utils;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
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
	public sealed class T4FileDependencyCache : T4PsiAwareCacheBase<T4FileDependencyData, T4FileDependencyData>,
		IT4FileDependencyGraph, IT4FileGraphNotifier
	{
		public event Action<IEnumerable<FileSystemPath>> OnFilesIndirectlyAffected;

		[NotNull]
		private IDictionary<FileSystemPath, T4FileDependencyData> IncluderToIncludes =>
			Map.Distinct(it => it.Key.GetLocation()).ToDictionary(it => it.Key.GetLocation(), it => it.Value);

		[NotNull]
		private IDictionary<FileSystemPath, T4FileDependencyData> IncludeToIncluders
		{
			get
			{
				var includerToIncludes = IncluderToIncludes;
				return includerToIncludes
					.Keys
					.Concat(Map.SelectMany(it => it.Value.Paths))
					.Distinct().ToDictionary(
						file => file,
						file => new T4FileDependencyData(includerToIncludes
							.Where(entry => entry.Value.Paths.Contains(file))
							.Select(entry => entry.Key).AsList()
						)
					);
			}
		}

		public T4FileDependencyCache(
			Lifetime lifetime,
			[NotNull] IPersistentIndexManager persistentIndexManager
		) : base(lifetime, persistentIndexManager, T4FileDependencyDataMarshaller.Instance)
		{
		}

		public IProjectFile FindBestRoot(IProjectFile file) =>
			FindBestRoot(file.Location).FindMostSuitableFile(file);

		[NotNull]
		private FileSystemPath FindBestRoot([NotNull] FileSystemPath include) =>
			new T4GraphSinkSearcher(IncludeToIncluders).FindClosestSink(include);

		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> FindIndirectIncludesTransitiveClosure([NotNull] FileSystemPath path) =>
			new T4IndirectIncludeTransitiveClosureSearcher(IncluderToIncludes, IncludeToIncluders).FindClosure(path);

		protected override T4FileDependencyData Build(IT4File file)
		{
			var includes = file
				.GetThisAndChildrenOfType<IT4IncludeDirective>()
				.Where(directive => directive.IsVisibleInDocument())
				.Select(directive => directive.Path.ResolvePath())
				.Where(path => !path.IsEmpty)
				.Distinct();
			return new T4FileDependencyData(includes.ToList());
		}

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			var oldIncludes = FindIndirectIncludesTransitiveClosure(sourceFile.GetLocation());
			base.Merge(sourceFile, builtPart);
			var newIncludes = FindIndirectIncludesTransitiveClosure(sourceFile.GetLocation());
			OnFilesIndirectlyAffected?.Invoke(oldIncludes.Concat(newIncludes));
		}
	}
}
