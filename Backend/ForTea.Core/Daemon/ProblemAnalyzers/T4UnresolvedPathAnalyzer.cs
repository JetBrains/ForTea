using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AssemblyDirective), HighlightingTypes =
		new[] {typeof(UnresolvedAssemblyWarning)})]
	public sealed class T4UnresolvedPathAnalyzer : T4AttributeValueProblemAnalyzerBase<IT4AssemblyDirective>
	{
		[NotNull]
		private IT4AssemblyReferenceResolver Resolver { get; }

		[NotNull]
		private IT4AssemblyNamePreprocessor Preprocessor { get; }

		public T4UnresolvedPathAnalyzer(
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] IT4AssemblyReferenceResolver resolver
		)
		{
			Preprocessor = preprocessor;
			Resolver = resolver;
		}

		protected override void DoRun(
			IT4AttributeValue element,
			IHighlightingConsumer consumer
		)
		{
			var t4File = element.GetContainingFile().NotNull();
			var sourceFile = t4File.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile();
			if (projectFile == null) return;
			using (T4MacroResolveContextCookie.Create(projectFile))
			using (Preprocessor.Prepare(projectFile))
			{
				string resolved = new T4PathWithMacros(element.GetText(), sourceFile).ResolveString();
				string path = Preprocessor.Preprocess(projectFile, resolved);
				var resolveContext = t4File.GetPsiModule().GetResolveContextEx(projectFile);
				var target = Resolver.FindAssemblyReferenceTarget(path);
				if (target == null)
				{
					consumer.AddHighlighting(new UnresolvedAssemblyWarning(element));
					return;
				}

				var fileSystemPath = Resolver.Resolve(target, projectFile.GetProject().NotNull(), resolveContext);
				if (fileSystemPath?.ExistsFile != true) consumer.AddHighlighting(new UnresolvedAssemblyWarning(element));
			}
		}

		protected override DirectiveAttributeInfo GetTargetAttribute() => T4DirectiveInfoManager.Assembly.NameAttribute;
	}
}
