using System;
using JetBrains.Annotations;
using JetBrains.DocumentManagers;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public class T4PsiProjectFile : PsiProjectFile, IIgnoredInSweaPsiSourceFile
	{
		public T4PsiProjectFile(
			[NotNull] IPsiModule psiModule,
			[NotNull] IProjectFile projectFile,
			[NotNull] Func<IProjectFile, IPsiSourceFile, IPsiSourceFileProperties> propertiesProvider,
			[NotNull] Func<IProjectFile, IPsiSourceFile, bool> validityChecks,
			DocumentManager documentManager,
			[NotNull] IModuleReferenceResolveContext resolveContext
		) : base(psiModule,
			projectFile,
			propertiesProvider,
			validityChecks,
			documentManager,
			resolveContext
		)
		{
		}
	}
}