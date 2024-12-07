using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
  [SolutionComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4IndirectIncludeTransitiveClosureSearcher
  {
    [NotNull] private IT4PsiFileSelector Selector { get; }

    public T4IndirectIncludeTransitiveClosureSearcher([NotNull] IT4PsiFileSelector selector) => Selector = selector;

    [NotNull, ItemNotNull]
    public JetHashSet<IPsiSourceFile> FindClosure(
      [NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
      [NotNull] Func<IPsiSourceFile, T4ReversedFileDependencyData> reversedProvider,
      [NotNull] IPsiSourceFile file
    ) => FindAllIncludes(provider, FindAllIncluders(reversedProvider, file));

    /// <summary>
    /// Performs DFS to collect all the files that include the current one,
    /// avoiding loops in includes if necessary
    /// </summary>
    [NotNull, ItemNotNull]
    private IEnumerable<IPsiSourceFile> FindAllIncluders(
      [NotNull] Func<IPsiSourceFile, T4ReversedFileDependencyData> reversedProvider,
      [NotNull] IPsiSourceFile file
    )
    {
      var result = new JetHashSet<IPsiSourceFile>();
      FindAllParents(file, reversedProvider, result);
      return result;
    }

    [NotNull, ItemNotNull]
    private JetHashSet<IPsiSourceFile> FindAllIncludes(
      [NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
      [NotNull, ItemNotNull] IEnumerable<IPsiSourceFile> includers
    )
    {
      var result = new JetHashSet<IPsiSourceFile>();
      foreach (var includer in includers)
      {
        FindAllChildren(includer, provider, result);
      }

      return result;
    }

    private void FindAllChildren(
      [NotNull] IPsiSourceFile file,
      [NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
      [NotNull, ItemNotNull] ISet<IPsiSourceFile> destination
    )
    {
      if (destination.Contains(file)) return;
      destination.Add(file);
      var data = provider(file);
      if (data == null) return;
      foreach (var sourceFile in data
                 .Includes
                 .Select(child => Selector.FindMostSuitableFile(child, file))
                 .Where(sourceFile => sourceFile != null)
              )
      {
        FindAllChildren(sourceFile, provider, destination);
      }
    }

    private void FindAllParents(
      [NotNull] IPsiSourceFile file,
      [NotNull] Func<IPsiSourceFile, T4ReversedFileDependencyData> reversedProvider,
      [NotNull, ItemNotNull] ISet<IPsiSourceFile> destination
    )
    {
      if (destination.Contains(file)) return;
      destination.Add(file);
      var data = reversedProvider(file);
      if (data == null) return;
      foreach (var sourceFile in data
                 .Includers
                 .Select(child => Selector.FindMostSuitableFile(child, file))
                 .Where(sourceFile => sourceFile != null)
              )
      {
        FindAllParents(sourceFile, reversedProvider, destination);
      }
    }
  }
}