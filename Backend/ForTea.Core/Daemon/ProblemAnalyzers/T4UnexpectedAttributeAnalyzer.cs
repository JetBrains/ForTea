using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4Token), HighlightingTypes =
		new[] {typeof(T4UnexpectedAttributeHighlighting)})]
	public sealed class T4UnexpectedAttributeAnalyzer : ElementProblemAnalyzer<IT4Directive>
	{
		protected override void Run(IT4Directive directive, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
		{
			var directiveInfo = T4DirectiveInfoManager.GetDirectiveByName(directive.Name.GetText());
			if (directiveInfo == null) return;

			var badAttributes = directive
				.Attributes
				.Where(attribute => directiveInfo.GetAttributeByName(attribute.Name.GetText()) == null);
			foreach (var attribute in badAttributes)
			{
				consumer.AddHighlighting(new T4UnexpectedAttributeHighlighting(attribute.Name));
			}
		}
	}
}
