using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;

using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Html;
using JetBrains.ReSharper.Psi.Html;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4StatementBlock), HighlightingTypes = new[] {typeof(StatementAfterFeatureError)})]
	[ZoneMarker(typeof(ILanguageHtmlZone))]
	public sealed class T4StatementAfterFeatureAnalyzer : ElementProblemAnalyzer<IT4StatementBlock>
	{
		protected override void Run(
			IT4StatementBlock element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			if (!element.PrevSiblings().Any(it => it is IT4FeatureBlock)) return;
			consumer.AddHighlighting(new StatementAfterFeatureError(element.Start));
		}
	}
}
