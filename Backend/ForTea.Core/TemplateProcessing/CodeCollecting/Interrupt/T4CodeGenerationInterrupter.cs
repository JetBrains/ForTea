namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt
{
	public sealed class T4CodeGenerationInterrupter : IT4CodeGenerationInterrupter
	{
		public void InterruptAfterProblem() => throw new T4OutputGenerationException();
	}
}
