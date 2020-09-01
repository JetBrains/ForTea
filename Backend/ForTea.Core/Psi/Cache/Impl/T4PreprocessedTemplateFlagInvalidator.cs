using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	[SolutionComponent]
	public sealed class T4PreprocessedTemplateFlagInvalidator : T4IndirectFileChangeObserverBase
	{
		[NotNull]
		private IT4RootTemplateKindProvider RootTemplateKindProvider { get; }

		[NotNull]
		private ChangeManager ChangeManager { get; }

		public T4PreprocessedTemplateFlagInvalidator(
			Lifetime lifetime,
			[NotNull] IT4FileGraphNotifier notifier,
			[NotNull] IPsiServices services,
			[NotNull] IPsiCachesState state,
			[NotNull] IT4RootTemplateKindProvider rootTemplateKindProvider,
			[NotNull] ChangeManager changeManager
		) : base(lifetime, notifier, services, state)
		{
			RootTemplateKindProvider = rootTemplateKindProvider;
			ChangeManager = changeManager;
		}

		protected override void AfterCommit()
		{
			using var cookie = ReadLockCookie.Create();
			foreach (var sourceFile in IndirectDependencies)
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

		private void UpdateFile([NotNull] IPsiSourceFile file)
		{
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(file.PsiModule, PsiModuleChange.ChangeType.Modified);
			var ShellLocks = file.GetSolution().Locks;
			var provider = SelectChangeProvider(file);
			if (provider == null) return;
			ShellLocks.ExecuteOrQueueEx(Lifetime.Eternal, "todo write reason", () =>
				ChangeManager.ExecuteAfterChange(
					() => ShellLocks.ExecuteWithWriteLock(
						() => ChangeManager.OnProviderChanged(
							provider,
							changeBuilder.Result,
							SimpleTaskExecutor.Instance
						)
					)
				));
		}

		[CanBeNull]
		private static IChangeProvider SelectChangeProvider([NotNull] IPsiSourceFile file)
		{
			// todo find a better change provider
			if (!(file.PsiModule is T4FilePsiModule t4Module)) return null;
			return t4Module.ChangeProvider;
		}

		protected override void OnFilesIndirectlyAffected(T4FileInvalidationData data)
		{
			IndirectDependencies.Add(data.DirectlyAffectedFile);
			Services.Locks.AssertMainThread();
			foreach (var file in data.IndirectlyAffectedFiles)
			{
				IndirectDependencies.Add(file);
			}
		}

		protected override string ActivityName => "T4 preprocessed flag invalidation";
	}
}
