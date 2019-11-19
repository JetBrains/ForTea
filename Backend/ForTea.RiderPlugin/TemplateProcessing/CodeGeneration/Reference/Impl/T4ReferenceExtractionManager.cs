using System.Collections.Generic;
using System.Linq;
using Debugger.Common.MetadataAndPdb;
using GammaJul.ForTea.Core;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using JetBrains.Util.dataStructures;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference.Impl
{
	[SolutionComponent]
	public sealed class T4ReferenceExtractionManager : IT4ReferenceExtractionManager
	{
		[NotNull]
		private RoslynMetadataReferenceCache Cache { get; }

		[NotNull]
		private IT4AssemblyReferenceResolver AssemblyReferenceResolver { get; }

		[NotNull]
		private IT4LowLevelReferenceExtractionManager LowLevelReferenceExtractionManager { get; }

		[NotNull]
		private IT4Environment Environment { get; }

		public T4ReferenceExtractionManager(
			Lifetime lifetime,
			[NotNull] IT4AssemblyReferenceResolver assemblyReferenceResolver,
			[NotNull] IT4LowLevelReferenceExtractionManager lowLevelReferenceExtractionManager,
			[NotNull] IT4Environment environment
		)
		{
			AssemblyReferenceResolver = assemblyReferenceResolver;
			LowLevelReferenceExtractionManager = lowLevelReferenceExtractionManager;
			Environment = environment;
			Cache = new RoslynMetadataReferenceCache(lifetime);
		}

		public IEnumerable<MetadataReference> ExtractPortableReferencesTransitive(Lifetime lifetime, IT4File file)
		{
			file.AssertContainsNoIncludeContext();
			var sourceFile = file.PhysicalPsiSourceFile.NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var directives = file.GetThisAndIncludedFilesRecursive()
				.SelectMany(it => it.Blocks)
				.OfType<IT4AssemblyDirective>();
			var errors = new FrugalLocalList<T4FailureRawData>();
			var directDependencies = directives.SelectNotNull(
				directive =>
				{
					var resolved = AssemblyReferenceResolver.Resolve(directive);
					if (resolved == null)
					{
						errors.Add(T4FailureRawData.FromElement(directive, "Unresolved assembly reference"));
					}

					return resolved;
				}
			).AsList();

			if (!errors.IsEmpty) throw new T4OutputGenerationException(errors);
			AddBaseReferences(directDependencies, file);
			var result = LowLevelReferenceExtractionManager.ResolveTransitiveDependencies(
				directDependencies,
				projectFile.SelectResolveContext()
			).Select(path => Cache.GetMetadataReference(lifetime, path)).AsList<MetadataReference>();
			return result;
		}

		private void AddBaseReferences(
			[NotNull, ItemNotNull] List<FileSystemPath> directDependencies,
			[NotNull] IT4File file
		)
		{
			directDependencies.Add(AddReference(file, "mscorlib"));
			directDependencies.Add(AddReference(file, "System"));
			directDependencies.AddRange(
				Environment.TextTemplatingAssemblyNames.Select(assemblyName => AddReference(file, assemblyName))
			);
		}

		[CanBeNull]
		private FileSystemPath AddReference(
			[NotNull] IT4File file,
			[NotNull] string assemblyName
		) => AssemblyReferenceResolver.Resolve(assemblyName, file.LogicalPsiSourceFile);

		public IEnumerable<T4AssemblyReferenceInfo> ExtractReferenceLocationsTransitive(IT4File file)
		{
			file.AssertContainsNoIncludeContext();
			var directReferences = ExtractRawAssemblyReferences(file);
			var sourceFile = file.LogicalPsiSourceFile.NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var resolveContext = projectFile.SelectResolveContext();
			return LowLevelReferenceExtractionManager
				.ResolveTransitiveDependencies(directReferences, resolveContext)
				.AsList();
		}

		[NotNull]
		private IEnumerable<T4AssemblyReferenceInfo> ExtractRawAssemblyReferences([NotNull] IT4File file)
		{
			var sourceFile = file.LogicalPsiSourceFile.NotNull();
			if (!(sourceFile.PsiModule is IT4FilePsiModule psiModule))
				return EmptyList<T4AssemblyReferenceInfo>.Enumerable;

			return psiModule
				.RawReferences
				.SelectNotNull(it => LowLevelReferenceExtractionManager.Resolve(it));
		}
	}
}
