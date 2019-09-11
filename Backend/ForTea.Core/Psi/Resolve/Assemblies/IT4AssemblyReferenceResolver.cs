using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	// TODO: inline IT4AssemblyNamePreprocessor
	public interface IT4AssemblyReferenceResolver
	{
		[CanBeNull]
		AssemblyReferenceTarget FindAssemblyReferenceTarget([NotNull] string assemblyNameOrFile);

		[CanBeNull]
		FileSystemPath Resolve(
			[NotNull] AssemblyReferenceTarget target,
			[NotNull] IProject project,
			[NotNull] IModuleReferenceResolveContext resolveContext);

		[CanBeNull]
		FileSystemPath Resolve([NotNull] IT4AssemblyDirective directive);

		[NotNull]
		IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
			[NotNull] IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			[NotNull] IProject project,
			[NotNull] IModuleReferenceResolveContext resolveContext);
	}
}
