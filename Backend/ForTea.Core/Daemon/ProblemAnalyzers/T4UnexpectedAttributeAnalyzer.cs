using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4Token), HighlightingTypes =
		new[] {typeof(T4UnexpectedAttributeHighlighting)})]
	public class T4UnexpectedAttributeAnalyzer : ElementProblemAnalyzer<IT4Token>
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4UnexpectedAttributeAnalyzer([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		protected override void Run(IT4Token element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
		{
			if (element.GetTokenType() != T4TokenNodeTypes.TOKEN) return;
			if (!(element.Parent is IT4DirectiveAttribute attribute)) return;
			Assertion.Assert(attribute.Parent is IT4Directive, "attribute.Parent is IT4Directive");
			var directive = (IT4Directive) attribute.Parent;
			var directiveInfo = Manager.GetDirectiveByName(directive.Name.GetText());
			if (directiveInfo == null) return;
			var attributeInfo = directiveInfo.GetAttributeByName(attribute.Name.GetText());
			if (attributeInfo != null) return;
			consumer.AddHighlighting(new T4UnexpectedAttributeHighlighting(element));
		}
	}
}
