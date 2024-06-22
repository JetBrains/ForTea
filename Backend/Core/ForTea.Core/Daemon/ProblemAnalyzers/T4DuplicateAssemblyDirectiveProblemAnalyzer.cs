using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(
    Instantiation.DemandAnyThreadUnsafe, typeof(IT4AssemblyDirective),
    HighlightingTypes = new[] { typeof(RedundantAssemblyWarning) }
  )]
  public sealed class T4DuplicateAssemblyDirectiveProblemAnalyzer : ElementProblemAnalyzer<IT4AssemblyDirective>
  {
    [NotNull] private IT4AssemblyReferenceResolver Resolver { get; }

    public T4DuplicateAssemblyDirectiveProblemAnalyzer([NotNull] IT4AssemblyReferenceResolver resolver) =>
      Resolver = resolver;

    protected override void Run(
      IT4AssemblyDirective element,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer consumer
    )
    {
      var elementStart = element.GetTreeStartOffset();
      var assemblyPath = Resolver.Resolve(element);
      if (assemblyPath == null || assemblyPath.IsEmpty) return;
      var earlierAssemblies = element
        .GetContainingFile()
        .NotNull()
        .GetThisAndChildrenOfType<IT4AssemblyDirective>()
        .Where(it => it.GetTreeStartOffset() < elementStart)
        .Where(it => Resolver.Resolve(it.ResolvedPath) == assemblyPath);
      if (!earlierAssemblies.Any()) return;
      consumer.AddHighlighting(new RedundantAssemblyWarning(element));
    }
  }
}