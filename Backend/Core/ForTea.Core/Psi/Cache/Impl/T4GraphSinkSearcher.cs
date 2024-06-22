using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Utils;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
  [SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
  public sealed class T4GraphSinkSearcher
  {
    [NotNull] private IT4PsiFileSelector Selector { get; }

    public T4GraphSinkSearcher([NotNull] IT4PsiFileSelector selector) => Selector = selector;

    /// <summary>
    /// Perform a breadth-first search for a sink.
    /// Since the graph is not guaranteed to contain no cycles,
    /// it also checks that there are no includes.
    /// If there are no potential sinks
    /// (if, for example, the source is a sink itself,
    /// or there are only loops in hierarchy),
    /// returns source.
    /// </summary>
    [NotNull]
    public IPsiSourceFile FindClosestSink(
      [NotNull] Func<IPsiSourceFile, T4ReversedFileDependencyData> provider,
      [NotNull] IPsiSourceFile source
    )
    {
      var guard = new T4IncludeGuard();
      guard.StartProcessing(source.GetLocation());
      ISet<IPsiSourceFile> currentLayer = new JetHashSet<IPsiSourceFile>(new[] { source });
      while (!currentLayer.IsEmpty())
      {
        var currentLayerSink = TrySelectSink(provider, currentLayer);
        if (currentLayerSink != null) return currentLayerSink;
        var previousLayer = currentLayer;
        currentLayer = previousLayer
          .SelectNotNull(file => provider(file)?.Includers
            .Select(path => Selector.FindMostSuitableFile(path, file)))
          .SelectMany(it => it)
          .SelectNotNull(it => it)
          .Where(path =>
          {
            bool canProcess = guard.CanProcess(path.GetLocation());
            if (canProcess) guard.StartProcessing(path.GetLocation());
            return canProcess;
          }).AsSet();
      }

      // So, there must be a recursion in includes.
      // This is not going to compile anyway,
      // so no support for this case
      return source;
    }

    [CanBeNull]
    private IPsiSourceFile TrySelectSink(
      [NotNull] Func<IPsiSourceFile, T4ReversedFileDependencyData> provider,
      [NotNull] ISet<IPsiSourceFile> candidates
    ) =>
      candidates.Where(file => IsSink(provider, file)).OrderBy(path => path.Name).FirstOrDefault();

    private bool IsSink(
      [NotNull] Func<IPsiSourceFile, T4ReversedFileDependencyData> provider,
      [NotNull] IPsiSourceFile vertex
    ) => provider(vertex)?.Includers.IsEmpty() != false;
  }
}