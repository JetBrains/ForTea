using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	public abstract class T4AttributeValueProblemAnalyzerBase<TDirective> : ElementProblemAnalyzer<TDirective>
		where TDirective : IT4Directive
	{
		protected sealed override void Run(
			TDirective element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			var values = element
				.GetAttributes(GetTargetAttribute())
				.SelectNotNull(it => it.Value);
			foreach (var value in values)
			{
				DoRun(value, consumer);
			}
		}

		[NotNull]
		protected abstract DirectiveAttributeInfo GetTargetAttribute();

		protected abstract void DoRun(
			[NotNull] IT4AttributeValue element,
			[NotNull] IHighlightingConsumer consumer);
	}
}
