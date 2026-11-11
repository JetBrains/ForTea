using JetBrains.Annotations;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.DocumentModel;
using JetBrains.Platform.RdFramework;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Search;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.ReSharperPlugin.Frontend
{
  [ZoneMarker(typeof(IDocumentModelZone), typeof(IProjectModelZone), typeof(IRdFrameworkZone))]
  internal static class T4ResolutionUtils
  {
    /// Can only be called from the UI thread
    [CanBeNull]
    public static IVsHierarchy TryGetVsHierarchy(
      [NotNull] ProjectModelSynchronizer synchronizer, [NotNull] IProjectModelElementSearchDescriptor project)
    {
      return synchronizer.TryGetHierarchyItemByProjectItem(project, false)
        ?.Hierarchy;
    }
  }
}
