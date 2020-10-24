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
	public static class T4CodeGeneration
	{
		[NotNull]
		public static T4CSharpCodeGenerationResult GenerateCodeBehind([NotNull] IT4File file)
		{
			var solution = file.GetSolution();
			var rootTemplateKindProvider = solution.GetComponent<IT4TemplateKindProvider>();
			var graph = solution.GetComponent<IT4FileDependencyGraph>();
			var root = graph.FindBestRoot(file.PhysicalPsiSourceFile.NotNull());
			T4CSharpCodeBehindGenerationInfoCollector collector;
			T4CSharpCodeBehindIntermediateConverter converter;
			var rootTemplateKind = rootTemplateKindProvider.GetTemplateKind(root);
			if (rootTemplateKind == T4TemplateKind.Preprocessed)
			{
				bool isRoot = root == file.PhysicalPsiSourceFile;
				collector = new T4PreprocessedCSharpCodeBehindGenerationInfoCollector(solution, root, graph, isRoot);
				var templateDataProvider = new T4PreprocessedClassNameProvider(root);
				converter = new T4CSharpCodeBehindIntermediateConverter(file, templateDataProvider, isRoot);
			}
			else
			{
				converter = new T4CSharpExecutableCodeBehindIntermediateConverter(file);
				collector = new T4CSharpCodeBehindGenerationInfoCollector(solution);
			}

			return converter.Convert(collector.Collect(file));
		}
	}
}
