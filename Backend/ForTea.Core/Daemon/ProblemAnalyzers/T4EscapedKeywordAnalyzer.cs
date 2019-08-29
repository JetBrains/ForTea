using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4ParameterDirective), HighlightingTypes = new[] {typeof(EscapedKeywordWarning)})]
	public sealed class T4EscapedKeywordAnalyzer : T4AttributeValueProblemAnalyzerBase<IT4ParameterDirective>
	{
		protected override DirectiveAttributeInfo GetTargetAttribute() =>
			T4DirectiveInfoManager.Parameter.TypeAttribute;

		protected override void DoRun(IT4AttributeValue element, IHighlightingConsumer consumer)
		{
			if (!CSharpLexer.IsKeyword(element.GetText())) return;
			consumer.AddHighlighting(new EscapedKeywordWarning(element));
		}
	}
}
