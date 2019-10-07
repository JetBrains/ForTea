using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Invalidation;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators
{
	/// <summary>
	/// This class generates a code-behind file
	/// from C# embedded statements and directives in the T4 file.
	/// That file is used for providing code highlighting and other code insights
	/// in T4 source file.
	/// That code is not intended to be compiled and run.
	/// </summary>
	internal sealed class T4CSharpCodeBehindGenerator : T4CSharpCodeGeneratorBase
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private T4FileDependencyManager DependencyManager { get; }

		public T4CSharpCodeBehindGenerator(
			[NotNull] IT4File actualFile,
			[NotNull] ISolution solution
		) : base(actualFile)
		{
			Solution = solution;
			DependencyManager = solution.GetComponent<T4FileDependencyManager>();
		}

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector =>
			new T4CSharpCodeBehindGenerationInfoCollector(Root, Solution);

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpCodeBehindIntermediateConverter(intermediateResult, ActualFile);

		/// <summary>
		/// In generated code-behind, we use root file to provide intelligent support for 
		/// </summary>
		[NotNull]
		private IT4File Root
		{
			get
			{
				var sourceLocation = ActualFile.GetSourceFile().GetLocation();
				var rootLocation = DependencyManager.Graph.FindBestRoot(sourceLocation);
				var rootPsiSourceFile = Solution
					.FindProjectItemsByLocation(rootLocation)
					.OfType<IProjectFile>()
					.SingleOrDefault()
					?.ToSourceFile();
				if (ActualFile.GetSourceFile() == rootPsiSourceFile)
					return ActualFile;

				var root = rootPsiSourceFile
					?.GetPsiFiles<T4Language>()
					.OfType<IT4File>()
					.Single();
				return root.NotNull();
			}
		}
	}
}
