using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4Directive), HighlightingTypes =
		new[] {typeof(UnexpectedAttributeWarning)})]
	public sealed class T4UnexpectedAttributeAnalyzer : ElementProblemAnalyzer<IT4Directive>
	{
		protected override void Run(IT4Directive directive, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
		{
			var directiveInfo = T4DirectiveInfoManager.GetDirectiveByName(directive.Name?.GetText());
			if (directiveInfo == null) return;

			var badAttributes = directive
				.Attributes
				.Where(attribute => directiveInfo.GetAttributeByName(attribute.Name.GetText()) == null);
			foreach (var attribute in badAttributes)
			{
				consumer.AddHighlighting(new UnexpectedAttributeWarning(attribute.Name));
			}
		}
	}
}
