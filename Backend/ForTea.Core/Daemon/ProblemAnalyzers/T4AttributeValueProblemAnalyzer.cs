using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	public abstract class T4AttributeValueProblemAnalyzer : ElementProblemAnalyzer<IT4AttributeValue>
	{
		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		protected T4AttributeValueProblemAnalyzer([NotNull] T4DirectiveInfoManager directiveInfoManager) =>
			DirectiveInfoManager = directiveInfoManager;

		protected sealed override void Run(
			IT4AttributeValue element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			Assertion.Assert(element.Parent is IT4DirectiveAttribute, "element.Parent is IT4DirectiveAttribute");
			var attribute = (IT4DirectiveAttribute) element.Parent;
			if (attribute.GetName() != GetTargetAttribute(DirectiveInfoManager).Name) return;
			Assertion.Assert(attribute.Parent is IT4Directive, "attribute.Parent is IT4Directive");
			var directive = (IT4Directive) attribute.Parent;
			if (!directive.IsSpecificDirective(GetTargetDirective(DirectiveInfoManager))) return;
			Assertion.Assert(directive.Parent is IT4File, "directive.Parent is IT4File");
			var t4File = (IT4File) directive.Parent;
			DoRun(element, consumer, directive, t4File);
		}

		[NotNull]
		protected abstract DirectiveInfo GetTargetDirective([NotNull] T4DirectiveInfoManager manager);

		[NotNull]
		protected abstract DirectiveAttributeInfo GetTargetAttribute([NotNull] T4DirectiveInfoManager manager);

		protected abstract void DoRun([NotNull] IT4AttributeValue element,
			[NotNull] IHighlightingConsumer consumer,
			[NotNull] IT4Directive directive,
			[NotNull] IT4File t4File);
	}
}
