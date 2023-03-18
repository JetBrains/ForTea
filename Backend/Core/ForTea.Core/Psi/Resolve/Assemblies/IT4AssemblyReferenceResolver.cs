using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
  public interface IT4AssemblyReferenceResolver
  {
    [CanBeNull]
    VirtualFileSystemPath Resolve([NotNull] IT4AssemblyDirective directive);

    [CanBeNull]
    VirtualFileSystemPath Resolve([NotNull] T4ResolvedPath path);

    [CanBeNull]
    VirtualFileSystemPath ResolveWithoutCaching([NotNull] T4ResolvedPath path);

    /// <note>
    /// assemblyName is assumed to NOT contain macros
    /// </note>
    [CanBeNull]
    VirtualFileSystemPath Resolve([NotNull] string assemblyNameOrFile, [NotNull] IPsiSourceFile sourceFile);
  }
}