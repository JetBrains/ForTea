using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public class T4ProjectReferenceResolver : IT4ProjectReferenceResolver
	{
		[NotNull]
		private IPsiModules PsiModules { get; }

		public T4ProjectReferenceResolver([NotNull] IPsiModules psiModules) => PsiModules = psiModules;

		public IEnumerable<IProject> GetProjectDependencies(IT4File file)
		{
			file.AssertContainsNoIncludeContext();
			var sourceFile = file.LogicalPsiSourceFile.NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var psiModule = sourceFile.PsiModule;
			var resolveContext = projectFile.SelectResolveContext();
			using (CompilationContextCookie.GetOrCreate(resolveContext))
			{
				return PsiModules
					.GetModuleReferences(psiModule)
					.Select(it => it.Module)
					.OfType<IProjectPsiModule>()
					.Select(it => it.Project)
					.AsList();
			}
		}
	}
}
