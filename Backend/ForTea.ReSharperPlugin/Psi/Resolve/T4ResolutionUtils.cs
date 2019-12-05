using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve
{
	internal static class T4ResolutionUtils
	{
		/// Can only be called from the UI thread
		[CanBeNull]
		public static IVsHierarchy TryGetVsHierarchy([NotNull] IProjectFile file) => file
			.GetSolution()
			.TryGetComponent<ProjectModelSynchronizer>()
			?.TryGetHierarchyItemByProjectItem(file.GetProject().NotNull().ToProjectSearchDescriptor(), false)
			?.Hierarchy;
	}
}
