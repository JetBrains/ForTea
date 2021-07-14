using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public class T4CSharpCodeGenerationInfoCollector : T4CSharpCodeGenerationInfoCollectorBase
	{
		protected override IT4CodeGenerationInterrupter Interrupter { get; } = new T4CodeGenerationInterrupter();

		public T4CSharpCodeGenerationInfoCollector([NotNull] ISolution solution) : base(solution)
		{
		}

		protected override void AppendTransformation(string message, IT4TreeNode firstNode)
		{
			if (Result.FeatureStarted) Result.AppendFeature(message, firstNode.NotNull());
			else Result.AppendTransformation(message);
		}

		protected override void AppendFeature(IT4Code code, IT4AppendableElementDescription description) =>
			Result.AppendFeature(description);
	}
}
