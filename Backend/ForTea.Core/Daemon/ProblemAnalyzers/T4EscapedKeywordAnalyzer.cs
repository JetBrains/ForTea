using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4Directive), HighlightingTypes = new[] {typeof(EscapedKeywordHighlighting)})]
	public class T4EscapedKeywordAnalyzer : T4AttributeValueProblemAnalyzerBase
	{
		public T4EscapedKeywordAnalyzer([NotNull] T4DirectiveInfoManager manager) : base(manager)
		{
		}

		protected override DirectiveInfo GetTargetDirective(T4DirectiveInfoManager manager) => manager.Parameter;

		protected override DirectiveAttributeInfo GetTargetAttribute(T4DirectiveInfoManager manager) =>
			manager.Parameter.TypeAttribute;

		protected override void DoRun(IT4AttributeValue element, IHighlightingConsumer consumer)
		{
			if (!CSharpLexer.IsKeyword(element.GetText())) return;
			consumer.AddHighlighting(new EscapedKeywordHighlighting(element));
		}
	}
}
