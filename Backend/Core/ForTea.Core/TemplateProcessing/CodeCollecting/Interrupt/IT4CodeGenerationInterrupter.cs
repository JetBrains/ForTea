namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt
{
  public interface IT4CodeGenerationInterrupter
  {
    void InterruptAfterProblem(T4FailureRawData failureData);
  }
}