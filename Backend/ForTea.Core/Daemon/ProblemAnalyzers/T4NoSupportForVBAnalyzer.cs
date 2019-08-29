using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4TemplateDirective), HighlightingTypes = new[] {typeof(NoSupportForVBHighlighting)})]
	public sealed class T4NoSupportForVBAnalyzer : T4AttributeValueProblemAnalyzerBase<IT4TemplateDirective>
	{
		protected override DirectiveAttributeInfo GetTargetAttribute() =>
			T4DirectiveInfoManager.Template.LanguageAttribute;

		protected override void DoRun(IT4AttributeValue element, IHighlightingConsumer consumer)
		{
			if (element.GetText() != "VB") return;
			consumer.AddHighlighting(new NoSupportForVBHighlighting(element));
		}
	}
}
