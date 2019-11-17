using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Utils;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
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
	public sealed class T4FileDependencyCache : SimpleICache<T4FileDependencyData>,
		IT4FileDependencyGraph, IT4FileGraphNotifier
	{
		[NotNull]
		private ILogger Logger { get; }

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
			[NotNull] IPersistentIndexManager persistentIndexManager,
			[NotNull] ILogger logger
		) : base(lifetime, persistentIndexManager, T4FileDependencyDataMarshaller.Instance) => Logger = logger;

		public IProjectFile FindBestRoot(IProjectFile file) =>
			FindBestRoot(file.Location).FindMostSuitableFile(file);

		[NotNull]
		private FileSystemPath FindBestRoot([NotNull] FileSystemPath include) =>
			new T4GraphSinkSearcher(IncludeToIncluders).FindClosestSink(include);

		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> FindIndirectIncludesTransitiveClosure([NotNull] FileSystemPath path) =>
			new T4IndirectIncludeTransitiveClosureSearcher(IncluderToIncludes, IncludeToIncluders).FindClosure(path);

		protected override bool IsApplicable(IPsiSourceFile sf)
		{
			if (!base.IsApplicable(sf)) return false;
			// While it is technically possible to include
			// any file (a C++ file, for example)
			// into a T4 file and still get some valid code,
			// we are definitely not supporting that case
			// because wtf nobody does like that
			return sf.LanguageType is T4ProjectFileType;
		}

		[NotNull]
		public override object Build(IPsiSourceFile sourceFile, bool isStartup)
		{
			// It is safe to access the PSI here.
			// According to SimpleICache documentation,
			// by the moment merge will be called,
			// PSI will have already been built
			var t4File = sourceFile.GetTheOnlyPsiFile<T4Language>() as IT4File;
			var includes = t4File
				.NotNull()
				.GetThisAndChildrenOfType<IT4IncludeDirective>()
				.Where(directive => directive.IsVisibleInDocument())
				.Select(directive => directive.Path.ResolvePath())
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
