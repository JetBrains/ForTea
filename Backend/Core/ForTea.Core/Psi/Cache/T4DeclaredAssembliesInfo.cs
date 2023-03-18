using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
  public sealed class T4DeclaredAssembliesInfo
  {
    [NotNull, ItemNotNull]
    private readonly JetHashSet<T4ResolvedPath> ReferencedAssemblies = new JetHashSet<T4ResolvedPath>();

    private void HandleAssemblyDirective([NotNull] IT4AssemblyDirective directive)
    {
      // i.e. non-empty
      var existingPath = directive.ResolvedPath;
      if (!existingPath.ResolvedPath.IsNotEmpty()) return;
      ReferencedAssemblies.Add(existingPath);
    }

    /// <summary>Computes a difference between this data and another one.</summary>
    /// <param name="oldDeclaredAssembliesInfo">The old data.</param>
    /// <returns>
    /// An instance of <see cref="T4DeclaredAssembliesDiff"/> containing the difference between the two data,
    /// or <c>null</c> if there are no differences.
    /// </returns>
    [CanBeNull]
    public T4DeclaredAssembliesDiff DiffWith([CanBeNull] T4DeclaredAssembliesInfo oldDeclaredAssembliesInfo)
    {
      if (oldDeclaredAssembliesInfo == null)
      {
        if (ReferencedAssemblies.Count == 0) return null;
        return new T4DeclaredAssembliesDiff(ReferencedAssemblies, EmptyList<T4ResolvedPath>.InstanceList);
      }

      oldDeclaredAssembliesInfo.ReferencedAssemblies.Compare(
        ReferencedAssemblies,
        out JetHashSet<T4ResolvedPath> addedAssemblies,
        out JetHashSet<T4ResolvedPath> removedAssemblies
      );

      if (addedAssemblies.Count == 0 && removedAssemblies.Count == 0) return null;
      return new T4DeclaredAssembliesDiff(addedAssemblies, removedAssemblies);
    }

    public T4DeclaredAssembliesInfo([NotNull] IT4File baseFile)
    {
      foreach (var directive in baseFile.GetThisAndChildrenOfType<IT4AssemblyDirective>())
      {
        HandleAssemblyDirective(directive);
      }
    }
  }
}