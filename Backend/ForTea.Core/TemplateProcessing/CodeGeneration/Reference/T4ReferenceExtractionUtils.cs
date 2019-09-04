using System.Collections.Generic;
using System.Linq;
using Debugger.Common.MetadataAndPdb;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference
{
	public static class T4ReferenceExtractionUtils
	{
		[NotNull]
		public static List<PortableExecutableReference> ExtractReferences(
			[NotNull] this IT4File file,
			Lifetime lifetime,
			[NotNull] IShellLocks locks,
			[NotNull] RoslynMetadataReferenceCache cache
		)
		{
			locks.AssertReadAccessAllowed();
			return ExtractRawAssemblyReferences(file)
				.Select(it => it.Location)
				.SelectNotNull(it => cache.GetMetadataReference(lifetime, it))
				.AsList();
		}

		[NotNull]
		public static List<T4AssemblyReferenceInfo> ExtractReferenceLocations([NotNull] this IT4File file) =>
			ExtractRawAssemblyReferences(file).Select(it => new T4AssemblyReferenceInfo(
				StringLiteralConverter.EscapeToRegular(it.AssemblyName?.FullName),
				StringLiteralConverter.EscapeToRegular(it.Location.FullPath))
			).AsList();

		[NotNull, ItemNotNull]
		private static IEnumerable<IAssemblyFile> ExtractRawAssemblyReferences([NotNull] this IT4File file)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			if (!(sourceFile.PsiModule is IT4FilePsiModule psiModule)) return EmptyList<IAssemblyFile>.Enumerable;
			var resolveContext = psiModule.GetResolveContextEx(projectFile);
			using (CompilationContextCookie.GetOrCreate(resolveContext))
			{
				return psiModule.RawReferences.SelectNotNull(it => it.GetFiles().Single()).AsList();
			}
		}
	}
}
