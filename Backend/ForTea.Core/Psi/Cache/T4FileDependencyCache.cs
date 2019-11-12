using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache.Impl;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	/// <summary>
	/// This cache stores the T4 file include dependency graph and manages dependent file invalidation:
	/// whenever a file is marked as dirty and its include list changes,
	/// this cache marks all the former dependencies and all the new dependencies as dirty.
	/// </summary>
	[PsiComponent]
	public sealed class T4FileDependencyCache : SimpleICache<T4FileDependencyData>, IT4FileDependencyGraph
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private IDictionary<FileSystemPath, T4FileDependencyData> IncluderToIncludes =>
			Map.ToDictionary(it => it.Key.GetLocation(), it => it.Value);

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

		[NotNull]
		private ISolution Solution { get; }

		public T4FileDependencyCache(
			Lifetime lifetime,
			[NotNull] IPersistentIndexManager persistentIndexManager,
			[NotNull] ILogger logger,
			[NotNull] ISolution solution
		) : base(lifetime, persistentIndexManager, T4FileDependencyDataMarshaller.Instance)
		{
			Logger = logger;
			Solution = solution;
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
		private FileSystemPath FindBestRoot([NotNull] FileSystemPath includee) =>
			new T4GraphSinkSearcher(IncludeToIncluders).FindClosestSink(includee);

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

		public override object Build(IPsiSourceFile sourceFile, bool isStartup)
		{
			// It is safe to access the PSI here.
			// According to SimpleICache documentation,
			// by the moment merge will be called,
			// PSI will have already been built
			if (!(sourceFile.GetTheOnlyPsiFile<T4Language>() is IT4File t4File)) return null;
			var includes = t4File
				.BlocksEnumerable
				.OfType<IT4IncludeDirective>()
				.Where(directive => directive.IsVisibleInDocument())
				.Select(directive => directive.Path.ResolvePath())
				.Distinct();
			return new T4FileDependencyData(includes.ToList());
		}

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			if (sourceFile.IsIndirectDependency() || builtPart == null)
			{
				base.Merge(sourceFile, builtPart);
				sourceFile.MarkAsIndependent();
				return;
			}

			var oldIncludes = FindIndirectIncludesTransitiveClosure(sourceFile.GetLocation());
			base.Merge(sourceFile, builtPart);
			var newIncludes = FindIndirectIncludesTransitiveClosure(sourceFile.GetLocation());
			var psiServices = sourceFile.GetPsiServices();

			// We want all files that were included before the update
			// and all the files that have become included now
			// to be updated, so we mark them as dirty
			var filesToInvalidate = oldIncludes
				.Concat(newIncludes)
				.Distinct()
				.SelectMany(Solution.FindProjectItemsByLocation)
				.OfType<IProjectFile>()
				.Select(psiServices.Modules.GetPsiSourceFilesFor)
				.SelectMany(sourceFiles => sourceFiles.AsEnumerable())
				// No need to bother if that file is dirty anyway
				.Where(file => !Dirty.Contains(file))
				.AsList();

			// After the merge, caches are expected to contain do dirty files, so delay the invalidation
			Solution.Locks.ExecuteOrQueueEx("T4 file dependency invalidation", () =>
			{
				using (WriteLockCookie.Create())
				{
					foreach (var file in filesToInvalidate)
					{
						// However, simply marking files as dirty causes infinite loops of updating,
						// so we also track whether the current file is being updated due to a document change
						// or due to indirect include invalidation
						file.MarkAsIndirectDependency();
						// When caches for that file get rebuilt,
						// they will not trigger a cascade of other updates
						psiServices.Files.MarkAsDirty(file);
						psiServices.Caches.MarkAsDirty(file);
					}
				}
			});
		}
	}
}
