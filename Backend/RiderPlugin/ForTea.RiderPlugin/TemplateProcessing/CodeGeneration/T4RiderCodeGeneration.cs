using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration
{
	public static class T4RiderCodeGeneration
	{
		[NotNull]
		public static T4CSharpCodeGenerationResult GenerateExecutableCode([NotNull] IT4File file)
		{
			var solution = file.GetSolution();
			var manager = solution.GetComponent<IT4ReferenceExtractionManager>();
			var collector = new T4CSharpCodeGenerationInfoCollector(solution);
			var converter = new T4CSharpExecutableIntermediateConverter(file, manager);
			return converter.Convert(collector.Collect(file));
		}

		/// <summary>
		/// This class preprocesses T4 file
		/// to produce C# file that can become part of the project
		/// and be compiled and run correctly.
		/// </summary>
		[NotNull]
		public static T4CSharpCodeGenerationResult GeneratePreprocessedCode([NotNull] IT4File file)
		{
			var solution = file.GetSolution();
			var collector = new T4CSharpPreprocessedCodeGenerationInfoCollector(solution);
			file.AssertContainsNoIncludeContext();
			var nameProvider = new T4PreprocessedClassNameProvider(file.PhysicalPsiSourceFile.NotNull());
			var converter = new T4CSharpRealIntermediateConverter(file, nameProvider);
			return converter.Convert(collector.Collect(file));
		}
	}
}
