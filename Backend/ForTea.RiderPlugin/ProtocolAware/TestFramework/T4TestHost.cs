using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Cache.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Core;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.TestFramework
{
	[SolutionComponent]
	public sealed class T4TestHost
	{
		public T4TestHost(
			[NotNull] T4TestModel model,
			[NotNull] SolutionsManager solutionsManager
		)
		{
			model.PreprocessFile.Set(location =>
			{
				using var cookie = ReadLockCookie.Create();
				var solution = solutionsManager.Solution;
				if (solution == null) return Unit.Instance;

				var host = solution.GetComponent<ProjectModelViewHost>();
				var projectFile = host.GetItemById<IProjectFile>(location.Id).NotNull();
				var sourceFile = projectFile.ToSourceFile().NotNull();
				sourceFile.GetPsiServices().Files.CommitAllDocuments();
				var file = sourceFile.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleItem().NotNull();

				var templatePreprocessingManager = solution.GetComponent<IT4TemplatePreprocessingManager>();
				templatePreprocessingManager.Preprocess(file);
				return Unit.Instance;
			});
			model.WaitForIndirectInvalidation.Set((lifetime, unit) =>
			{
				using var cookie = ReadLockCookie.Create();
				var solution = solutionsManager.Solution;
				if (solution == null) return RdTask<Unit>.Successful(Unit.Instance);
				var cache = solution.GetComponent<T4FileDependencyCache>();

				var result = new RdTask<Unit>();

				void Requeue(int n)
				{
					if (n == 1)
					{
						result.Set(Unit.Instance);
						return;
					}
					solution.Locks.Queue(lifetime, "T4TestHost::.ctor::lambda::Requeue", () =>
					{
						if (cache.HasDirtyFiles) Commit(n - 1);
						else Requeue(n - 1);
					});
				}

				void Commit(int n) => solution.GetPsiServices().Files.ExecuteAfterCommitAllDocuments(() => Requeue(n));
				// First commit applies all changes in files.
				// Then we re-queue the action to make sure that indirect invalidation happens,
				// then we commit the files again to apply the indirect changes
				// Commit(2) -> Requeue(2) -> Commit(1) -> Requeue(1)
				Commit(2);

				return result;
			});
		}
	}
}
