using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Invalidation.Impl;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation
{
	/// <summary>
	/// Manages the include dependencies between T4 files,
	/// to be able to refresh includers when the included files change.
	/// Is effectively a wrapper that helps to update <see cref="T4FileDependencyGraph">the real graph</see>
	/// </summary>
	[PsiComponent]
	public sealed class T4FileDependencyManager
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private object Locker { get; } = new object();

		[NotNull]
		public IT4FileDependencyGraph Graph { get; } = new T4FileDependencyGraph();

		[NotNull]
		private IPsiServices PsiServices { get; }

		[CanBeNull]
		private T4FileDependencyInvalidator Invalidator { get; set; }

		[NotNull]
		private HashSet<FileSystemPath> FilesBeingCommitted { get; } = new HashSet<FileSystemPath>();

		public T4FileDependencyManager(Lifetime lifetime, [NotNull] IPsiServices psiServices, [NotNull] ILogger logger)
		{
			PsiServices = psiServices;
			Logger = logger;
			psiServices.Files.ObserveBeforeCommit(lifetime, OnBeforeFilesCommit);
			psiServices.Files.ObserveAfterCommit(lifetime, OnAfterFilesCommit);
		}

		public void UpdateIncludes([NotNull] FileSystemPath includer, [NotNull] ICollection<FileSystemPath> includees)
		{
			lock (Locker)
			{
				Graph.UpdateIncludes(includer, includees);
			}
		}

		[CanBeNull]
		internal T4FileDependencyInvalidator TryGetCurrentInvalidator()
		{
			lock (Locker)
			{
				return Invalidator;
			}
		}

		[NotNull]
		public IEnumerable<FileSystemPath> GetIncluders([NotNull] FileSystemPath includee)
		{
			lock (Locker)
			{
				return new HashSet<FileSystemPath>(Graph.GetIncluders(includee));
			}
		}

		private void OnAfterFilesCommit()
		{
			Logger.Verbose("OnAfterFilesCommit");
			T4FileDependencyInvalidator invalidator;
			lock (Locker)
			{
				invalidator = Invalidator;
				Invalidator = null;
			}

			if (invalidator == null) return;
			invalidator.CommittedFilePaths.RemoveWhere(FilesBeingCommitted.Contains);
			FilesBeingCommitted.AddRange(invalidator.CommittedFilePaths);
			invalidator.CommitNeededDocuments();
		}

		private void OnBeforeFilesCommit()
		{
			var invalidator = new T4FileDependencyInvalidator(this, PsiServices);
			lock (Locker)
			{
				Invalidator = invalidator;
			}
		}
	}
}
