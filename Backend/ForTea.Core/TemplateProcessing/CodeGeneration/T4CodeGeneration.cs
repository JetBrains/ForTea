using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>
	/// This class generates a code-behind text
	/// from C# embedded statements and directives in the T4 file.
	/// That text is used for providing code highlighting and other code insights
	/// in T4 source file.
	/// That code is not intended to be compiled and run.
	/// </summary>
	// TODO: make it a component
	public static class T4CodeGeneration
	{
		[NotNull]
		public static T4CSharpCodeGenerationResult GenerateCodeBehind([NotNull] IT4File file)
		{
			var solution = file.GetSolution();
			var rootTemplateKindProvider = solution.GetComponent<IT4TemplateKindProvider>();
			var graph = solution.GetComponent<IT4FileDependencyGraph>();
			var root = graph.FindBestRoot(file.PhysicalPsiSourceFile.NotNull());
			T4CSharpCodeBehindIntermediateConverter converter;
			var rootTemplateKind = rootTemplateKindProvider.GetTemplateKind(root);
			if (rootTemplateKind == T4TemplateKind.Preprocessed)
			{
				var nameProvider = new T4PreprocessedClassNameProvider(root);
				converter = new T4CSharpCodeBehindIntermediateConverter(file, nameProvider);
			}
			else converter = new T4CSharpExecutableCodeBehindIntermediateConverter(file);
			var collector = new T4CSharpCodeBehindGenerationInfoCollector(solution, rootTemplateKind);
			// todo: do not generate base class for includes
			// TODO: store files incuded into preprocessed files in the project PSI module
			return converter.Convert(collector.Collect(file));
		}
	}
}
