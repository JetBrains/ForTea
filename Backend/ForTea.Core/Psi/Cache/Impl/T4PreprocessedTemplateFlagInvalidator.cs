using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Transaction;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	[SolutionComponent]
	public sealed class T4PreprocessedTemplateFlagInvalidator : T4IndirectFileChangeObserverBase
	{
		[NotNull]
		private IT4RootTemplateKindProvider RootTemplateKindProvider { get; }

		[NotNull]
		private ChangeManager ChangeManager { get; }

		[NotNull]
		private ISolution Solution { get; }

		public T4PreprocessedTemplateFlagInvalidator(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state,
			[NotNull] IT4RootTemplateKindProvider rootTemplateKindProvider,
			[NotNull] ChangeManager changeManager,
			[NotNull] ISolution solution
		) : base(lifetime, notifier, services, state)
		{
			RootTemplateKindProvider = rootTemplateKindProvider;
			ChangeManager = changeManager;
			Solution = solution;
		}

		protected override void AfterCommitSync(ISet<IPsiSourceFile> indirectDependencies)
		{
			using var cookie = ReadLockCookie.Create();
			foreach (var sourceFile in indirectDependencies)
			{
				var projectFile = sourceFile.ToProjectFile();
				if (projectFile == null) continue;
				if (RootTemplateKindProvider.IsRootPreprocessedTemplate(sourceFile))
				{
					if (projectFile.IsFlaggedAsPreprocessed()) continue;
					projectFile.FlagAsPreprocessed();
					UpdateFile(sourceFile);
				}
				else
				{
					if (!projectFile.IsFlaggedAsPreprocessed()) continue;
					projectFile.FlagAsExecutable();
					UpdateFile(sourceFile);
				}
			}
		}

		private void UpdateFile([NotNull] IPsiSourceFile file) => ChangeManager.ExecuteAfterChange(() =>
		{
			using var cookie = WriteLockCookie.Create();
			var oldParentItem = file.GetProject();
			var projectFile = file.ToProjectFile().NotNull();
			var changeDelta = new ProjectItemChange(
				ProjectModelChange.EMPTY_DELTAS, projectFile, oldParentItem,
				ProjectModelChangeType.UNKNOWN, projectFile.Location,
				ExternalChangeType.NONE, projectFile.GetPersistentID()).Propagate();
			ProjectModelChangeUtil.OnChange(Solution.BatchChangeManager, changeDelta);
		});

		protected override void OnFilesIndirectlyAffected(
			T4FileInvalidationData data,
			ISet<IPsiSourceFile> indirectDependencies
		)
		{
			indirectDependencies.Add(data.DirectlyAffectedFile);
			Services.Locks.AssertMainThread();
			foreach (var file in data.IndirectlyAffectedFiles)
			{
				indirectDependencies.Add(file);
			}
		}

		protected override string ActivityName => "T4 preprocessed flag invalidation";
	}
}
