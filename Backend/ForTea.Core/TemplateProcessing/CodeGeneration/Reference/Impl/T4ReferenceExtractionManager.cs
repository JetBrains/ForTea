using System.Collections.Generic;
using System.Linq;
using Debugger.Common.MetadataAndPdb;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference.Impl
{
	[SolutionComponent]
	public sealed class T4ReferenceExtractionManager : IT4ReferenceExtractionManager
	{
		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private RoslynMetadataReferenceCache Cache { get; }

		[NotNull]
		private IPsiModules PsiModules { get; }

		[NotNull]
		private IT4AssemblyReferenceResolver AssemblyReferenceResolver { get; }

		public T4ReferenceExtractionManager(
			Lifetime lifetime,
			[NotNull] IShellLocks locks,
			[NotNull] IPsiModules psiModules,
			[NotNull] IT4AssemblyReferenceResolver assemblyReferenceResolver
		)
		{
			Locks = locks;
			PsiModules = psiModules;
			AssemblyReferenceResolver = assemblyReferenceResolver;
			Cache = new RoslynMetadataReferenceCache(lifetime);
		}

		public IEnumerable<PortableExecutableReference> ExtractReferences(IT4File file, Lifetime lifetime)
		{
			Locks.AssertReadAccessAllowed();
			return ExtractRawAssemblyReferences(file)
				.Select(it => it.Location)
				.SelectNotNull(it => Cache.GetMetadataReference(lifetime, it))
				.AsList();
		}

		public IEnumerable<T4AssemblyReferenceInfo> ExtractReferenceLocationsTransitive(IT4File file)
		{
			var directReferences = ExtractRawAssemblyReferences(file).Select(assemblyFile =>
				new T4AssemblyReferenceInfo(assemblyFile.AssemblyName?.FullName ?? "", assemblyFile.Location)
			);
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var project = projectFile.GetProject().NotNull();
			var psiModule = sourceFile.PsiModule;
			var resolveContext = psiModule.GetResolveContextEx(projectFile);
			return AssemblyReferenceResolver
				.ResolveTransitiveDependencies(directReferences, project, resolveContext)
				.AsList();
		}

		[NotNull, ItemNotNull]
		private static IEnumerable<IAssemblyFile> ExtractRawAssemblyReferences([NotNull] IT4File file)
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

		public IEnumerable<IProject> GetProjectDependencies(IT4File file)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var psiModule = sourceFile.PsiModule;
			var resolveContext = psiModule.GetResolveContextEx(projectFile);
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
