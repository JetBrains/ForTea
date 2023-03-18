using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  /// <summary>
  /// Cache holding <see cref="T4DeclaredAssembliesInfo"/> for each T4 file.
  /// It cannot be implemented like ordinary cache because it needs PSI to build.
  /// </summary>
  [PsiComponent]
  public class T4DeclaredAssembliesManager
  {
    [NotNull] private IPsiFiles PsiFiles { get; }

    [NotNull] private IShellLocks Locks { get; }

    [NotNull] public Signal<Pair<IPsiSourceFile, T4DeclaredAssembliesDiff>> FileDataChanged { get; }

    public T4DeclaredAssembliesManager(
      Lifetime lifetime,
      [NotNull] IPsiFiles psiFiles,
      [NotNull] IShellLocks locks
    )
    {
      PsiFiles = psiFiles;
      Locks = locks;
      FileDataChanged = new Signal<Pair<IPsiSourceFile, T4DeclaredAssembliesDiff>>(
        lifetime,
        "T4DeclaredAssembliesCache.FileDataChanged"
      );
      lifetime.Bracket(
        () => psiFiles.PsiFileCreated += OnPsiFileChanged,
        () => psiFiles.PsiFileCreated -= OnPsiFileChanged
      );
      lifetime.Bracket(
        () => psiFiles.AfterPsiChanged += OnPsiChanged,
        () => psiFiles.AfterPsiChanged -= OnPsiChanged
      );
    }

    /// <summary>Called when a PSI file is created.</summary>
    /// <param name="file">The file that was created.</param>
    private void OnPsiFileChanged([CanBeNull] IFile file)
    {
      if (!(file is IT4File t4File)) return;
      CreateOrUpdateData(t4File);
    }

    /// <summary>Called when a PSI element changes.</summary>
    /// <param name="treeNode">The tree node that changed.</param>
    /// <param name="psiChangedElementType">The type of the PSI change.</param>
    private void OnPsiChanged([CanBeNull] ITreeNode treeNode, PsiChangedElementType psiChangedElementType)
    {
      if (treeNode == null) return;
      if (psiChangedElementType != PsiChangedElementType.SourceContentsChanged) return;
      OnPsiFileChanged(treeNode.GetContainingFile());
    }

    // Omitting CommitAllDocuments causes races between caches and assembly resolver
    protected virtual void CreateOrUpdateData([NotNull] IT4File t4File)
    {
      Locks.ExecuteOrQueueEx("T4 assembly reference invalidation", () =>
      {
        if (!t4File.IsValid()) return;
        using var cookie = ReadLockCookie.Create();
        PsiFiles.ExecuteAfterCommitAllDocuments(() => DoInvalidateAssemblies(t4File));
      });
    }

    protected void DoInvalidateAssemblies([NotNull] IT4File t4File)
    {
      var sourceFile = t4File.PhysicalPsiSourceFile;
      if (sourceFile?.IsValid() != true) return;
      if (!sourceFile.LanguageType.Is<T4ProjectFileType>()) return;
      var newData = new T4DeclaredAssembliesInfo(t4File);
      var existingDeclaredAssembliesInfo = sourceFile.GetDeclaredAssembliesInfo();
      sourceFile.SetDeclaredAssembliesInfo(newData);
      var diff = newData.DiffWith(existingDeclaredAssembliesInfo);
      if (diff == null) return;
      FileDataChanged.Fire(Pair.Of(sourceFile, diff));
    }
  }
}