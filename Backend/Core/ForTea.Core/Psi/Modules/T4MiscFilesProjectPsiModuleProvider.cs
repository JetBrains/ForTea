using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules
{
  /// <summary>Provides <see cref="IT4FilePsiModule"/> for T4 files opened outside of the solution.</summary>
  [MiscFilesProjectPsiModuleProvider(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4MiscFilesProjectPsiModuleProvider : IMiscFilesProjectPsiModuleProvider
  {
    [NotNull] private readonly T4PsiModuleProvider _t4PsiModuleProvider;

    [NotNull]
    public IEnumerable<IPsiModule> GetModules() =>
      _t4PsiModuleProvider.GetModules();

    [NotNull]
    public IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile) =>
      _t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile);

    public void Dispose() => _t4PsiModuleProvider.Dispose();

    public void OnProjectFileChanged(
      [NotNull] IProjectFile projectFile,
      PsiModuleChange.ChangeType changeType,
      [NotNull] PsiModuleChangeBuilder changeBuilder,
      [NotNull] VirtualFileSystemPath oldLocation
    ) => _t4PsiModuleProvider.OnProjectFileChanged(projectFile, changeType, changeBuilder);

    public T4MiscFilesProjectPsiModuleProvider(
      Lifetime lifetime,
      [NotNull] IShellLocks shellLocks,
      [NotNull] ChangeManager changeManager,
      [NotNull] IT4Environment t4Environment,
      [NotNull] PsiProjectFileTypeCoordinator coordinator,
      [NotNull] IT4TemplateKindProvider templateDataManager
    ) => _t4PsiModuleProvider = new T4PsiModuleProvider(
      lifetime,
      shellLocks,
      changeManager,
      t4Environment,
      templateDataManager
    );
  }
}