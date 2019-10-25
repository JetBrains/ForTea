using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ReSharper.Psi;
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

		/// <note>
		/// assemblyName is assumed to NOT contain macros
		/// </note>
		[CanBeNull]
		FileSystemPath Resolve([NotNull] string assemblyNameOrFile, [NotNull] IPsiSourceFile sourceFile);

		[NotNull]
		IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
			[NotNull] IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext);
	}
}
