using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation.Impl
{
	[SolutionComponent]
	public sealed class T4IndirectIncludeInvalidator : IT4IndirectIncludeInvalidator
	{
		[NotNull]
		private T4FileDependencyManager DependencyManager { get; }

		[NotNull]
		private IPsiServices PsiServices { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4IndirectIncludeInvalidator(
			[NotNull] T4FileDependencyManager dependencyManager,
			[NotNull] IPsiServices psiServices,
			[NotNull] ISolution solution,
			[NotNull] ILogger logger
		)
		{
			DependencyManager = dependencyManager;
			PsiServices = psiServices;
			Solution = solution;
			Logger = logger;
		}

		public void InvalidateIndirectIncludes(FileSystemPath updatedFile)
		{
			var dirties = DependencyManager
				.Graph
				.FindIndirectIncludesTransitiveClosure(updatedFile)
				.SelectMany(dirtyLocation => Solution
					.FindProjectItemsByLocation(dirtyLocation)
					.OfType<IProjectFile>()
				);
			foreach (var dirty in dirties)
			{
				Logger.Verbose("Update in {0} => dirty {1}", updatedFile.Name, dirty.Name);
				PsiServices.MarkAsDirty(dirty);
			}
		}
	}
}
