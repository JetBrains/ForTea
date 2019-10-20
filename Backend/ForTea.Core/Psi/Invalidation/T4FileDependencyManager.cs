using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Invalidation.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
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
		private T4CommitStage _stage = T4CommitStage.UserChangeApplication;

		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private object Locker { get; } = new object();

		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		[NotNull]
		private T4DeclaredAssembliesManager DeclaredAssembliesManager { get; }

		[CanBeNull]
		private T4FileDependencyInvalidator Invalidator { get; set; }

		[NotNull]
		private IShellLocks Locks { get; }

		private T4CommitStage Stage
		{
			get
			{
				Locks.AssertMainThread();
				return _stage;
			}
			set
			{
				Locks.AssertMainThread();
				_stage = value;
			}
		}

		public T4FileDependencyManager(
			Lifetime lifetime,
			[NotNull] IPsiServices psiServices,
			[NotNull] ILogger logger,
			[NotNull] IShellLocks locks,
			[NotNull] T4DeclaredAssembliesManager declaredAssembliesManager,
			[NotNull] IT4FileDependencyGraph graph
		)
		{
			PsiServices = psiServices;
			Logger = logger;
			Locks = locks;
			DeclaredAssembliesManager = declaredAssembliesManager;
			Graph = graph;
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

		private void OnAfterFilesCommit()
		{
			Logger.Verbose("OnAfterFilesCommit, Stage = {0}", Stage);
			if (Stage == T4CommitStage.DependencyInvalidation) return;
			T4FileDependencyInvalidator invalidator;
			lock (Locker)
			{
				invalidator = Invalidator;
				Invalidator = null;
			}

			Stage = T4CommitStage.DependencyInvalidation;
			invalidator?.CommitNeededDocuments();
			Stage = T4CommitStage.UserChangeApplication;
		}

		private void OnBeforeFilesCommit()
		{
			Logger.Verbose("OnBeforeFilesCommit. Stage = {0}", Stage);
			switch (Stage)
			{
				case T4CommitStage.UserChangeApplication:
					lock (Locker)
					{
						Invalidator = new T4FileDependencyInvalidator(PsiServices, Graph, DeclaredAssembliesManager);
					}

					return;
				case T4CommitStage.DependencyInvalidation:
					return;
			}
		}
	}
}
