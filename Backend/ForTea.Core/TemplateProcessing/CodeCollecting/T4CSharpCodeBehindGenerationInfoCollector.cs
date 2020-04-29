using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpCodeBehindGenerationInfoCollector : T4CSharpCodeGenerationInfoCollectorBase
	{
		protected override IT4CodeGenerationInterrupter Interrupter { get; } = new T4CodeBehindGenerationInterrupter();

		public T4CSharpCodeBehindGenerationInfoCollector([NotNull] ISolution solution) : base(solution)
		{
		}

		// There's no way tokens can code blocks, so there's no need to insert them into code behind
		protected override void AppendTransformation(string message)
		{
		}
	}
}
