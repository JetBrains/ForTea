using GammaJul.ForTea.Core.Psi.Cache;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services.Impl
{
  [SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
  public sealed class T4RootTemplateKindProvider : IT4RootTemplateKindProvider
  {
    [NotNull] private IT4TemplateKindProvider TemplateKindProvider { get; }

    [NotNull] private IT4FileDependencyGraph Graph { get; }

    public T4RootTemplateKindProvider(
      [NotNull] IT4TemplateKindProvider templateKindProvider,
      [NotNull] IT4FileDependencyGraph graph
    )
    {
      TemplateKindProvider = templateKindProvider;
      Graph = graph;
    }

    public T4TemplateKind GetRootTemplateKind(IPsiSourceFile file) =>
      TemplateKindProvider.GetTemplateKind(Graph.FindBestRoot(file));
  }
}