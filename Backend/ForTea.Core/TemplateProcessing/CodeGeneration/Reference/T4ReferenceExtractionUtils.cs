using System.Collections.Generic;
using System.Linq;
using Debugger.Common.MetadataAndPdb;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference
{
	public static class T4ReferenceExtractionUtils
	{
		public static List<MetadataReference> ExtractReferences(
			[NotNull] this IT4File file,
			Lifetime lifetime,
			[NotNull] IShellLocks locks,
			[NotNull] IPsiModules psiModules,
			[NotNull] RoslynMetadataReferenceCache cache
		)
		{
			locks.AssertReadAccessAllowed();
			return ExtractPsiReferences(file, psiModules)
				.SelectNotNull(it => it.Location)
				.SelectNotNull(it => cache.GetMetadataReference(lifetime, it))
				.AsList();
		}

		public static List<T4AssemblyReferenceInfo> ExtractReferenceLocations(
			[NotNull] this IT4File file,
			[NotNull] IPsiModules psiModules
		) => ExtractPsiReferences(file, psiModules)
			.Where(it => it.Location != null)
			.Select(it => new T4AssemblyReferenceInfo(
				StringLiteralConverter.EscapeToRegular(it.AssemblyName.FullName),
				StringLiteralConverter.EscapeToRegular(it.Location.FullPath))
			).AsList();

		private static List<IPsiAssembly> ExtractPsiReferences(
			[NotNull] this IT4File file,
			[NotNull] IPsiModules psiModules
		)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var psiModule = sourceFile.PsiModule;
			var resolveContext = psiModule.GetResolveContextEx(projectFile);
			using (CompilationContextCookie.GetOrCreate(resolveContext))
			{
				return psiModules
					.GetModuleReferences(psiModule)
					.Select(it => it.Module)
					.OfType<IAssemblyPsiModule>()
					.Select(it => it.Assembly)
					.AsList();
			}
		}
	}
}
