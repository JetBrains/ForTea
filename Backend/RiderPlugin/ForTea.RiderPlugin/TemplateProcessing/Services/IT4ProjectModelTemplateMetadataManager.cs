using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services
{
  public interface IT4ProjectModelTemplateMetadataManager
  {
    void UpdateTemplateMetadata(
      [NotNull] IProjectModelTransactionCookie cookie,
      [NotNull] IProjectFile template,
      T4TemplateKind kind,
      [CanBeNull] VirtualFileSystemPath outputLocation = null);

    void UpdateGeneratedFileMetadata(
      [NotNull] IProjectModelTransactionCookie cookie,
      [NotNull] IProjectFile generatedFile,
      [NotNull] IProjectFile template);

    [NotNull, ItemNotNull]
    IEnumerable<IProjectFile> FindLastGenOutput([NotNull] IProjectFile file);
  }
}