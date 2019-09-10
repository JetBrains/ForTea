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
	}
}
