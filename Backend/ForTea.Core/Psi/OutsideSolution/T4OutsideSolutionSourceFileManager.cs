using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using JetBrains.Application.FileSystemTracker;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Modules.ExternalFileModules;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
	/// <summary>A component that manages <see cref="IDocument"/>s for files outside the solution.</summary>
	[SolutionComponent]
	internal sealed class T4OutsideSolutionSourceFileManager : IPsiModuleFactory, IDisposable
	{
		// This might cause some memory leaks on IPsiSourceFile.
		// Can be replaced with reference counting if that issue turns out to be important
		[NotNull]
		private ConcurrentDictionary<FileSystemPath, IPsiSourceFile> SourceFiles { get; }

		[NotNull]
		private IProjectFileExtensions ProjectFileExtensions { get; }

		[NotNull]
		private PsiProjectFileTypeCoordinator PsiProjectFileTypeCoordinator { get; }

		[NotNull]
		private DocumentManager DocumentManager { get; }

		[NotNull]
		private IPsiModule PsiModule { get; }

		public HybridCollection<IPsiModule> Modules => new HybridCollection<IPsiModule>(PsiModule);

		public T4OutsideSolutionSourceFileManager(Lifetime lifetime,
			[NotNull] IProjectFileExtensions projectFileExtensions,
			[NotNull] PsiProjectFileTypeCoordinator psiProjectFileTypeCoordinator,
			[NotNull] DocumentManager documentManager,
			[NotNull] ISolution solution,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IFileSystemTracker fileSystemTracker
		)
		{
			ProjectFileExtensions = projectFileExtensions;
			PsiProjectFileTypeCoordinator = psiProjectFileTypeCoordinator;
			DocumentManager = documentManager;
			SourceFiles = new ConcurrentDictionary<FileSystemPath, IPsiSourceFile>();
			lifetime.OnTermination(this);
			PsiModule = new PsiModuleOnFileSystemPaths(
				solution,
				"T4OutsideSolution",
				Guid.NewGuid().ToString(),
				t4Environment.TargetFrameworkId,
				fileSystemTracker,
				lifetime,
				false);
		}

		[NotNull]
		public IPsiSourceFile GetOrCreateSourceFile([NotNull] FileSystemPath path)
		{
			Assertion.Assert(path.IsAbsolute, "path.IsAbsolute");
			return SourceFiles.GetOrAdd(path, _ => new T4OutsideSolutionSourceFile(
				ProjectFileExtensions,
				PsiProjectFileTypeCoordinator,
				PsiModule,
				path,
				sf => sf.Location.ExistsFile,
				sf => new T4OutsideSolutionSourceFileProperties(),
				DocumentManager,
				EmptyResolveContext.Instance)
			);
		}

		public bool HasSourceFile([NotNull] FileSystemPath path) => SourceFiles.ContainsKey(path);

		public void DeleteSourceFile([NotNull] FileSystemPath path)
		{
			SourceFiles.TryRemove(path, out var _);
		}

		public void Dispose() => SourceFiles.Clear();
	}
}
