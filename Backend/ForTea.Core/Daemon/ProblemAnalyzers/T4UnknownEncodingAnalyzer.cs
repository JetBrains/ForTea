using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AttributeValue), HighlightingTypes =
		new[] {typeof(T4UnknownEncodingHighlighting)})]
	public class T4UnknownEncodingAnalyzer : T4AttributeValueProblemAnalyzer
	{
		[NotNull]
		private T4EncodingsManager Manager { get; }

		public T4UnknownEncodingAnalyzer(
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] T4EncodingsManager manager
		) : base(directiveInfoManager) => Manager = manager;

		protected override void DoRun(
			IT4AttributeValue element,
			IHighlightingConsumer consumer,
			IT4Directive directive,
			IT4File t4File
		) => Manager.FindEncoding(directive, new T4ActionAdapterInterrupter(_ =>
			consumer.AddHighlighting(new T4UnknownEncodingHighlighting(element))
		));

		protected override DirectiveInfo GetTargetDirective(T4DirectiveInfoManager manager) => manager.Output;

		protected override DirectiveAttributeInfo GetTargetAttribute(T4DirectiveInfoManager manager) =>
			manager.Output.EncodingAttribute;
	}
}
