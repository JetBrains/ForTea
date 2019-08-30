using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation
{
	/// <summary>
	/// Manages the include dependencies between T4 files,
	/// to be able to refresh includers when the included files change.
	/// </summary>
	[PsiComponent]
	public class T4FileDependencyManager
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private object Locker { get; } = new object();

		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> IncluderToIncludees { get; } =
			new OneToSetMap<FileSystemPath, FileSystemPath>();

		[NotNull]
		private OneToSetMap<FileSystemPath, FileSystemPath> IncludeeToIncluders { get; } =
			new OneToSetMap<FileSystemPath, FileSystemPath>();

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
				foreach (var includee in IncluderToIncludees[includer])
				{
					IncludeeToIncluders.Remove(includee, includer);
				}

				IncluderToIncludees.RemoveKey(includer);
				if (includees.Count <= 0) return;
				IncluderToIncludees.AddRange(includer, includees);
				foreach (var includee in includees)
				{
					IncludeeToIncluders.Add(includee, includer);
				}
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
		public HashSet<FileSystemPath> GetIncluders([NotNull] FileSystemPath includee)
		{
			lock (Locker)
			{
				return new HashSet<FileSystemPath>(IncludeeToIncluders[includee]);
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
