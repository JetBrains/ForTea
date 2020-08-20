using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpCodeBehindGenerationInfoCollector : T4CSharpCodeGenerationInfoCollectorBase
	{
		private T4TemplateKind RootTemplateKind { get; }
		protected override IT4CodeGenerationInterrupter Interrupter { get; } = new T4CodeBehindGenerationInterrupter();

		public T4CSharpCodeBehindGenerationInfoCollector(
			[NotNull] ISolution solution,
			T4TemplateKind rootTemplateKind
		) : base(solution) => RootTemplateKind = rootTemplateKind;

		// There's no way tokens can code blocks, so there's no need to insert them into code behind
		protected override void AppendTransformation(string message)
		{
		}

		protected override void AppendFeature(IT4Code code, IT4AppendableElementDescription description)
		{
			if (RootTemplateKind == T4TemplateKind.Preprocessed && !code.IsVisibleInDocumentUnsafe()) return;
			Result.AppendFeature(description);
		}
	}
}
