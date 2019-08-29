using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
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
		private IModuleReferenceResolveManager ResolveManager { get; }

		[NotNull]
		private IT4AssemblyNamePreprocessor Preprocessor { get; }

		public T4UnresolvedPathAnalyzer(
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] IModuleReferenceResolveManager resolveManager
		)
		{
			Preprocessor = preprocessor;
			ResolveManager = resolveManager;
		}

		protected override void DoRun(
			IT4AttributeValue element,
			IHighlightingConsumer consumer
		)
		{
			var t4File = element.GetContainingFile().NotNull();
			var sourceFile = t4File.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			using (Preprocessor.Prepare(projectFile))
			{
				string resolved = new T4PathWithMacros(element.GetText(), sourceFile).ResolveString();
				string path = Preprocessor.Preprocess(projectFile, resolved);
				var resolveContext = t4File.GetPsiModule().GetResolveContextEx(projectFile);
				var target = T4AssemblyReferenceManager.FindAssemblyReferenceTarget(path);
				if (target == null)
				{
					consumer.AddHighlighting(new UnresolvedAssemblyWarning(element));
					return;
				}

				var fileSystemPath = ResolveManager.Resolve(target, projectFile.GetProject(), resolveContext);
				if (fileSystemPath?.ExistsFile != true) consumer.AddHighlighting(new UnresolvedAssemblyWarning(element));
			}
		}

		protected override DirectiveAttributeInfo GetTargetAttribute() => T4DirectiveInfoManager.Assembly.NameAttribute;
	}
}
