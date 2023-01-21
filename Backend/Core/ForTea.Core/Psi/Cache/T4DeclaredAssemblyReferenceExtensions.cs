using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  public static class T4DeclaredAssemblyReferenceExtensions
  {
    [NotNull]
    private static Key<T4DeclaredAssembliesInfo> DeclaredAssembliesInfoKey { get; } =
      new Key<T4DeclaredAssembliesInfo>("DeclaredAssembliesInfoKey");

    public static void SetDeclaredAssembliesInfo(
      [NotNull] this IPsiSourceFile file,
      [NotNull] T4DeclaredAssembliesInfo fileSystemPath
    ) => file.PutData(DeclaredAssembliesInfoKey, fileSystemPath);

    [CanBeNull]
    public static T4DeclaredAssembliesInfo GetDeclaredAssembliesInfo([NotNull] this IPsiSourceFile document) =>
      document.GetData(DeclaredAssembliesInfoKey);
  }
}