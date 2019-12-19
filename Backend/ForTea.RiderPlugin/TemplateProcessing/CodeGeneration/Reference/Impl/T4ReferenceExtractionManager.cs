using System.Collections.Generic;
using System.Linq;
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
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.dataStructures;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference.Impl
{
	[SolutionComponent]
	public sealed class T4ReferenceExtractionManager : IT4ReferenceExtractionManager
	{
		[NotNull]
		private IT4AssemblyReferenceResolver AssemblyReferenceResolver { get; }

		[NotNull]
		private IT4LowLevelReferenceExtractionManager LowLevelReferenceExtractionManager { get; }

		[NotNull]
		private IT4Environment Environment { get; }

		public T4ReferenceExtractionManager(
			[NotNull] IT4AssemblyReferenceResolver assemblyReferenceResolver,
			[NotNull] IT4LowLevelReferenceExtractionManager lowLevelReferenceExtractionManager,
			[NotNull] IT4Environment environment
		)
		{
			AssemblyReferenceResolver = assemblyReferenceResolver;
			LowLevelReferenceExtractionManager = lowLevelReferenceExtractionManager;
			Environment = environment;
		}

		public IEnumerable<MetadataReference> ExtractPortableReferencesTransitive(Lifetime lifetime, IT4File file) =>
			ExtractReferenceLocationsTransitive(file)
				.Select(info => LowLevelReferenceExtractionManager.ResolveMetadata(lifetime, info.Location))
				.AsList();

		public IEnumerable<T4AssemblyReferenceInfo> ExtractReferenceLocationsTransitive(IT4File file)
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
			return LowLevelReferenceExtractionManager.ResolveTransitiveDependencies(
				directDependencies,
				projectFile.SelectResolveContext()
			);
		}

		private void AddBaseReferences(
			[NotNull, ItemNotNull] List<FileSystemPath> directDependencies,
			[NotNull] IT4File file
		)
		{
			directDependencies.AddRange(
				Environment.DefaultAssemblyNames.Select(assemblyName => ResolveReference(file, assemblyName))
			);
		}

		[NotNull]
		private FileSystemPath ResolveReference(
			[NotNull] IT4File file,
			[NotNull] string assemblyName
		)
		{
			var resolved = AssemblyReferenceResolver.Resolve(assemblyName, file.LogicalPsiSourceFile);
			if (resolved != null) return resolved;
			var node = FindSuitableNodeForErrorReporting(file);
			string message = $"Could not resolve assembly: {assemblyName}";
			throw new T4OutputGenerationException(T4FailureRawData.FromElement(node, message));
		}

		[NotNull]
		private ITreeNode FindSuitableNodeForErrorReporting([NotNull] IT4File file)
		{
			ITreeNode templateDirective = file.BlocksEnumerable.OfType<IT4TemplateDirective>().FirstOrDefault();
			return templateDirective ?? file;
		}
	}
}
