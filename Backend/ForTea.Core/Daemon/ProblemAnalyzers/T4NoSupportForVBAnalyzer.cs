using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB;

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
			if (!(element.Parent is IT4DirectiveAttribute attribute)) return;
			if (!(attribute.Parent is T4Directive directive)) return;
			if (!directive.IsSpecificDirective(Manager.Template)) return;
			if (attribute.GetName() != Manager.Template.LanguageAttribute.Name) return;
			if (!Manager.GetLanguageType(element.GetContainingFile() as IT4File).Is<VBLanguage>()) return;
			consumer.AddHighlighting(new NoSupportForVBHighlighting(element));
		}
	}
}
