using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Caches.SymbolCache;
using JetBrains.ReSharper.Resources.Shell;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public class T4BasicOutputFileRefresher : IT4OutputFileRefresher
	{
		protected ISolution Solution { get; }
		public T4BasicOutputFileRefresher(ISolution solution) => Solution = solution;

		public virtual void Refresh(IProjectFile output)
		{
			var destinationSourceFile = output.ToSourceFile();
			if (destinationSourceFile == null) return;
			var changeManager = Solution.GetPsiServices().GetComponent<ChangeManager>();
			var invalidateCacheChange = new InvalidateCacheChange(
				Solution.GetComponent<SymbolCache>(),
				new[] {destinationSourceFile},
				true);

			using (WriteLockCookie.Create())
			{
				changeManager.OnProviderChanged(Solution, invalidateCacheChange, SimpleTaskExecutor.Instance);
			}
		}
	}
}
