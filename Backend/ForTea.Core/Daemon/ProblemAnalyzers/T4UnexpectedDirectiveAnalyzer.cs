using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4Directive), HighlightingTypes =
		new[] {typeof(T4UnexpectedDirectiveHighlighting)})]
	public class T4UnexpectedDirectiveAnalyzer : ElementProblemAnalyzer<IT4Directive>
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4UnexpectedDirectiveAnalyzer([NotNull] T4DirectiveInfoManager manager) => Manager = manager;
		
		protected override void Run(
			IT4Directive element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			if (Manager.GetDirectiveByName(element.GetName()) != null) return;
			var nameToken = element.GetNameToken().NotNull();
			consumer.AddHighlighting(new T4UnexpectedDirectiveHighlighting(nameToken));
		}
	}
}
