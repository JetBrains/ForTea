using System;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
	internal sealed class T4OutsideSolutionSourceFile : NavigateablePsiSourceFileWithLocation, IPsiSourceFile
	{
		public new IDocument Document
		{
			get
			{
				var document = base.Document;
				document.SetOutsideSolutionPath(Location);
				return document;
			}
		}

		public T4OutsideSolutionSourceFile(
			IProjectFileExtensions projectFileExtensions,
			PsiProjectFileTypeCoordinator projectFileTypeCoordinator,
			IPsiModule module,
			VirtualFileSystemPath path,
			Func<PsiSourceFileFromPath, bool> validityCheck,
			Func<PsiSourceFileFromPath, IPsiSourceFileProperties> propertiesFactory,
			DocumentManager documentManager,
			IModuleReferenceResolveContext resolveContext
		) : base(
			projectFileExtensions,
			projectFileTypeCoordinator,
			module,
			path,
			validityCheck,
			propertiesFactory,
			documentManager,
			resolveContext
		)
		{
		}
	}
}
