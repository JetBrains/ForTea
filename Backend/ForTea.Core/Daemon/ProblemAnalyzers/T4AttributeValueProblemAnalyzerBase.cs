using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	public abstract class T4AttributeValueProblemAnalyzerBase : ElementProblemAnalyzer<IT4Directive>
	{
		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		protected T4AttributeValueProblemAnalyzerBase([NotNull] T4DirectiveInfoManager directiveInfoManager) =>
			DirectiveInfoManager = directiveInfoManager;

		protected sealed override void Run(
			IT4Directive element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			if (!element.IsSpecificDirective(GetTargetDirective(DirectiveInfoManager))) return;
			var values = element
				.GetAttributes()
				.Where(it => it.GetName() == GetTargetAttribute(DirectiveInfoManager).Name)
				.SelectNotNull(it => it.GetValueToken());
			foreach (var value in values)
			{
				DoRun(value, consumer);
			}
		}

		[NotNull]
		protected abstract DirectiveInfo GetTargetDirective([NotNull] T4DirectiveInfoManager manager);

		[NotNull]
		protected abstract DirectiveAttributeInfo GetTargetAttribute([NotNull] T4DirectiveInfoManager manager);

		protected abstract void DoRun(
			[NotNull] IT4AttributeValue element,
			[NotNull] IHighlightingConsumer consumer);
	}
}
