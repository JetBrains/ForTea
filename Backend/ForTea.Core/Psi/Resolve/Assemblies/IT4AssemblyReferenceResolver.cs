using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	// TODO: inline IT4AssemblyNamePreprocessor
	public interface IT4AssemblyReferenceResolver
	{
		[CanBeNull]
		FileSystemPath Resolve([NotNull] IT4AssemblyDirective directive);

		[CanBeNull]
		FileSystemPath Resolve(IT4PathWithMacros path);

		/// <note>
		/// assemblyName is assumed to NOT contain macros
		/// </note>
		[CanBeNull]
		FileSystemPath Resolve([NotNull] string assemblyNameOrFile, [NotNull] IPsiSourceFile sourceFile);
	}
}
