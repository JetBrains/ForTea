using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(typeof(IT4Macro), HighlightingTypes = new[] { typeof(UnresolvedMacroError) })]
  public class T4UnresolvedMacroAnalyzer : ElementProblemAnalyzer<IT4Macro>
  {
    [NotNull] private IT4MacroResolver Resolver { get; }

    public T4UnresolvedMacroAnalyzer([NotNull] IT4MacroResolver resolver) => Resolver = resolver;

    protected override void Run(IT4Macro macro, ElementProblemAnalyzerData data, IHighlightingConsumer context)
    {
      var projectFile = macro.GetSourceFile().NotNull().ToProjectFile().NotNull();
      string name = macro.RawAttributeValue?.GetText();
      if (name == null) return;
      var macros = Resolver.ResolveHeavyMacros(new[] { name }, projectFile);
      if (macros.ContainsKey(name)) return;
      if (Resolver.IsSupported(macro)) context.AddHighlighting(new UnresolvedMacroError(macro));
      else context.AddHighlighting(new UnsupportedMacroError(macro));
    }
  }
}