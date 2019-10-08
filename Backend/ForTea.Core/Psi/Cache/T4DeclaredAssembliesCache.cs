using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	/// <summary>
	/// Cache holding <see cref="T4DeclaredAssembliesInfo"/> for each T4 file.
	/// It cannot be implemented like ordinary cache because it needs PSI to build.
	/// </summary>
	[PsiComponent]
	public sealed class T4DeclaredAssembliesCache
	{
		[NotNull]
		private WeakToStrongDictionary<IPsiSourceFile, T4DeclaredAssembliesInfo> DeclaredAssemblyInfos { get; } =
			new WeakToStrongDictionary<IPsiSourceFile, T4DeclaredAssembliesInfo>();

		[NotNull]
		public Signal<Pair<IPsiSourceFile, T4DeclaredAssembliesDiff>> FileDataChanged { get; }

		public T4DeclaredAssembliesCache(Lifetime lifetime, [NotNull] PsiFiles psiFiles)
		{
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
			lifetime.OnTermination(DeclaredAssemblyInfos);
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

		private void CreateOrUpdateData([NotNull] IT4File t4File)
		{
			var sourceFile = t4File.GetSourceFile();
			if (sourceFile?.LanguageType.Is<T4ProjectFileType>() != true) return;
			var newData = new T4DeclaredAssembliesInfo(t4File);
			T4DeclaredAssembliesInfo existingDeclaredAssembliesInfo;
			lock (DeclaredAssemblyInfos)
			{
				DeclaredAssemblyInfos.TryGetValue(sourceFile, out existingDeclaredAssembliesInfo);
				DeclaredAssemblyInfos[sourceFile] = newData;
			}

			var diff = newData.DiffWith(existingDeclaredAssembliesInfo);
			if (diff == null) return;
			FileDataChanged.Fire(Pair.Of(sourceFile, diff));
		}
	}
}
