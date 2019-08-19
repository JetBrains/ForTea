using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4Directive), HighlightingTypes = new[] {typeof(EscapedKeywordHighlighting)})]
	public class T4NoSupportForVBAnalyzer : T4AttributeValueProblemAnalyzerBase
	{
		public T4NoSupportForVBAnalyzer([NotNull] T4DirectiveInfoManager manager) : base(manager)
		{
		}

		protected override DirectiveInfo GetTargetDirective(T4DirectiveInfoManager manager) => manager.Template;

		protected override DirectiveAttributeInfo GetTargetAttribute(T4DirectiveInfoManager manager) =>
			manager.Template.LanguageAttribute;

		protected override void DoRun(IT4AttributeValue element, IHighlightingConsumer consumer)
		{
			if (element.GetText() != "VB") return;
			consumer.AddHighlighting(new NoSupportForVBHighlighting(element));
		}
	}
}
