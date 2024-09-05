using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
  [ElementProblemAnalyzer(Instantiation.DemandAnyThreadSafe, typeof(IT4EnvironmentVariable),
    HighlightingTypes = new[] { typeof(UnresolvedEnvironmentVariableError) }
  )]
  public class T4UnresolvedEnvironmentVariableAnalyzer : ElementProblemAnalyzer<IT4EnvironmentVariable>
  {
    protected override void Run(
      IT4EnvironmentVariable variable,
      ElementProblemAnalyzerData data,
      IHighlightingConsumer context
    )
    {
      var value = variable.RawAttributeValue;
      if (value == null) return;
      if (Environment.GetEnvironmentVariable(value.GetText()) != null) return;
      context.AddHighlighting(new UnresolvedEnvironmentVariableError(variable));
    }
  }
}