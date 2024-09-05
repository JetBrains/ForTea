using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.Parts;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  public abstract class T4IgnoredDirectiveInPreprocessedTemplateAnalyzer<TDirective> :
    ElementProblemAnalyzer<TDirective>
    where TDirective : IT4Directive
  {
    protected override void Run(TDirective element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
      var sourceFile = element.GetSourceFile().NotNull();
      var solution = sourceFile.GetSolution();
      var templateKindProvider = solution.GetComponent<IT4RootTemplateKindProvider>();
      if (!templateKindProvider.IsRootPreprocessedTemplate(sourceFile)) return;
      consumer.AddHighlighting(new IgnoredDirectiveWarning(element));
    }
  }

  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadSafe, typeof(IT4AssemblyDirective), HighlightingTypes =
    new[] { typeof(IgnoredDirectiveWarning) })]
  public sealed class T4IgnoredAssemblyDirectiveAnalyzer :
    T4IgnoredDirectiveInPreprocessedTemplateAnalyzer<IT4AssemblyDirective>
  {
  }

  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadSafe, typeof(IT4OutputDirective), HighlightingTypes =
    new[] { typeof(IgnoredDirectiveWarning) })]
  public sealed class T4IgnoredOutputDirectiveAnalyzer :
    T4IgnoredDirectiveInPreprocessedTemplateAnalyzer<IT4OutputDirective>
  {
  }
}