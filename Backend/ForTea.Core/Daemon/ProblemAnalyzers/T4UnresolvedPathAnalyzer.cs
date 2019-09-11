using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AssemblyDirective), HighlightingTypes =
		new[] {typeof(UnresolvedAssemblyWarning)})]
	public sealed class T4UnresolvedPathAnalyzer : T4AttributeValueProblemAnalyzerBase<IT4AssemblyDirective>
	{
		[NotNull]
		private IT4AssemblyReferenceResolver Resolver { get; }

		public T4UnresolvedPathAnalyzer([NotNull] IT4AssemblyReferenceResolver resolver) => Resolver = resolver;

		protected override void DoRun(IT4AttributeValue element, IHighlightingConsumer consumer)
		{
			var attribute = DirectiveAttributeNavigator.GetByValue(element);
			if (!(DirectiveNavigator.GetByAttribute(attribute) is IT4AssemblyDirective assemblyDirective)) return;
			var systemPath = Resolver.Resolve(assemblyDirective);
			if (systemPath == null || !systemPath.ExistsFile)
			{
				consumer.AddHighlighting(new UnresolvedAssemblyWarning(element));
			}
		}

		protected override DirectiveAttributeInfo GetTargetAttribute() => T4DirectiveInfoManager.Assembly.NameAttribute;
	}
}
