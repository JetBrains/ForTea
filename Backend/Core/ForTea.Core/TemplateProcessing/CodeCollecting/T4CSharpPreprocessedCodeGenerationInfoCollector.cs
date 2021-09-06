using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpPreprocessedCodeGenerationInfoCollector : T4CSharpCodeGenerationInfoCollector
	{
		public T4CSharpPreprocessedCodeGenerationInfoCollector([NotNull] ISolution solution) : base(solution)
		{
		}

		public override void VisitImportDirectiveNode(IT4ImportDirective importDirectiveParam)
		{
			var description = T4ImportDescription.FromDirective(importDirectiveParam);
			if (description == null) return;
			Result.Append(description);
		}
	}
}
