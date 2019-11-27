using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace JetBrains.ForTea.RiderPlugin.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AssemblyDirective), HighlightingTypes = new[] {typeof(NoSupportForEnvDteError)})]
	public sealed class T4NoSupportForEnvDteAnalyzer : ElementProblemAnalyzer<IT4AssemblyDirective>
	{
		protected override void Run(
			IT4AssemblyDirective element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			var attribute = element.GetFirstAttribute(T4DirectiveInfoManager.Assembly.NameAttribute);
			var attributeValue = attribute?.Value;
			if (attributeValue == null) return;
			if (!attributeValue.GetText().StartsWith("EnvDTE", StringComparison.OrdinalIgnoreCase)) return;
			consumer.AddHighlighting(new NoSupportForEnvDteError(attributeValue));
		}
	}
}
