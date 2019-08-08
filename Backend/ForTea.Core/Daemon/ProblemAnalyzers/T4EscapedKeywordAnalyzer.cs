using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AttributeValue), HighlightingTypes = new[] {typeof(EscapedKeywordHighlighting)})]
	public class T4EscapedKeywordAnalyzer : ElementProblemAnalyzer<IT4AttributeValue>
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4EscapedKeywordAnalyzer([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		protected override void Run(IT4AttributeValue token, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
		{
			Assertion.Assert(token.Parent is IT4DirectiveAttribute, "token.Parent is IT4DirectiveAttribute");
			var attribute = (IT4DirectiveAttribute) token.Parent;
			Assertion.Assert(attribute.Parent is IT4Directive, "attribute.Parent is IT4Directive");
			var directive = (IT4Directive) attribute.Parent;
			if (!directive.IsSpecificDirective(Manager.Parameter)) return;
			if (attribute.GetName() != Manager.Parameter.TypeAttribute.Name) return;
			if (!CSharpLexer.IsKeyword(token.GetText())) return;
			consumer.AddHighlighting(new EscapedKeywordHighlighting(token));
		}
	}
}
