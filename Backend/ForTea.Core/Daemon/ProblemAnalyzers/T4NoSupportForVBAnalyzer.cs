using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AttributeValue), HighlightingTypes = new[] {typeof(EscapedKeywordHighlighting)})]
	public class T4NoSupportForVBAnalyzer : ElementProblemAnalyzer<IT4AttributeValue>
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4NoSupportForVBAnalyzer([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		protected override void Run(
			IT4AttributeValue element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			Assertion.Assert(element.Parent is IT4DirectiveAttribute, "element.Parent is IT4DirectiveAttribute");
			var attribute = (IT4DirectiveAttribute) element.Parent;
			if (attribute.GetName() != Manager.Template.LanguageAttribute.Name) return;
			Assertion.Assert(attribute.Parent is IT4Directive, "attribute.Parent is T4Directive");
			var directive = (IT4Directive) attribute.Parent;
			if (!directive.IsSpecificDirective(Manager.Template)) return;
			if (element.GetText() != "VB") return;
			consumer.AddHighlighting(new NoSupportForVBHighlighting(element));
		}
	}
}
