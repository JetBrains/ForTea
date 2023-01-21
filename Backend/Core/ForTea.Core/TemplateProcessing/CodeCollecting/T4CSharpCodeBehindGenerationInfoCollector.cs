using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
  public class T4CSharpCodeBehindGenerationInfoCollector : T4CSharpCodeGenerationInfoCollectorBase
  {
    protected override IT4CodeGenerationInterrupter Interrupter { get; } = new T4CodeBehindGenerationInterrupter();

    public T4CSharpCodeBehindGenerationInfoCollector([NotNull] ISolution solution) : base(solution)
    {
    }

    // There's no way tokens can code blocks, so there's no need to insert them into code behind
    protected override void AppendTransformation(string message, IT4TreeNode firstNode)
    {
    }

    protected override void AppendFeature(IT4Code code, IT4AppendableElementDescription description)
    {
      // The simple case: all executable files reside in separate PSI modules,
      // so we generate complete code for them no matter what
      Result.AppendFeature(description);
    }
  }
}