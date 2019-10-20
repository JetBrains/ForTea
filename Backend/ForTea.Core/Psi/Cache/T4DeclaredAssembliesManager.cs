using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.Invalidation;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	[PsiComponent]
	public sealed class T4DeclaredAssembliesManager
	{
		[NotNull]
		private IT4FileDependencyGraph Graph { get; }

		[NotNull]
		public Signal<Pair<IPsiSourceFile, T4DeclaredAssembliesDiff>> FileDataChanged { get; }

		public T4DeclaredAssembliesManager(Lifetime lifetime, [NotNull] IT4FileDependencyGraph graph)
		{
			Graph = graph;
			FileDataChanged = new Signal<Pair<IPsiSourceFile, T4DeclaredAssembliesDiff>>(
				lifetime,
				"T4DeclaredAssembliesManager.FileDataChanged"
			);
		}

		public void UpdateReferences([NotNull, ItemNotNull] IEnumerable<IProjectFile> targets)
		{
			// This method should only be called on UI thread
			// after all the documents have been committed,
			// so it must be safe to access the PSI
			var files = targets
				.Select(PsiSourceFileExtensions.ToSourceFile)
				.Select(sourceFile => sourceFile.GetPrimaryPsiFile())
				.OfType<IT4File>();
			foreach (var file in files)
			{
				CreateOrUpdateData(file);
			}
		}

		private void CreateOrUpdateData([NotNull] IT4File t4File)
		{
			var sourceFile = t4File.GetSourceFile();
			if (sourceFile?.LanguageType.Is<T4ProjectFileType>() != true) return;
			var newData = new T4DeclaredAssembliesInfo(t4File, Graph);
			var existingDeclaredAssembliesInfo = sourceFile.GetDeclaredAssembliesInfo();
			sourceFile.SetDeclaredAssembliesInfo(newData);
			var diff = newData.DiffWith(existingDeclaredAssembliesInfo);
			if (diff == null) return;
			FileDataChanged.Fire(Pair.Of(sourceFile, diff));
		}
	}
}
