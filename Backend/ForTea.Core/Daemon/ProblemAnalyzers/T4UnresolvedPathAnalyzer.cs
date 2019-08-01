using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4AttributeValue), HighlightingTypes =
		// We could have handled include directives here, too,
		// but it would be way more cumbersome than in T4IncludeAwareDaemonProcessVisitor
		new[] {typeof(T4UnresolvedAssemblyHighlighting)})]
	public class T4UnresolvedPathAnalyzer : ElementProblemAnalyzer<IT4AttributeValue>
	{
		[NotNull]
		private IModuleReferenceResolveManager ResolveManager { get; }

		[NotNull]
		private IT4AssemblyNamePreprocessor Preprocessor { get; }

		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		public T4UnresolvedPathAnalyzer(
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] IModuleReferenceResolveManager resolveManager
		)
		{
			DirectiveInfoManager = directiveInfoManager;
			Preprocessor = preprocessor;
			ResolveManager = resolveManager;
		}

		protected override void Run(
			IT4AttributeValue element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			Assertion.Assert(element.Parent is IT4DirectiveAttribute, "element.Parent is IT4DirectiveAttribute");
			var attribute = (IT4DirectiveAttribute) element.Parent;
			Assertion.Assert(attribute.Parent is IT4Directive, "attribute.Parent is IT4Directive");
			var directive = (IT4Directive) attribute.Parent;
			if (!directive.IsSpecificDirective(DirectiveInfoManager.Assembly)) return;
			Assertion.Assert(directive.Parent is IT4File, "directive.Parent is IT4File");
			var t4File = (IT4File) directive.Parent;
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
					consumer.AddHighlighting(new T4UnresolvedAssemblyHighlighting(element));
					return;
				}

				var fileSystemPath = ResolveManager.Resolve(target, projectFile.GetProject(), resolveContext);
				if (fileSystemPath == null) consumer.AddHighlighting(new T4UnresolvedAssemblyHighlighting(element));
			}
		}
	}
}
