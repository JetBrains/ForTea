using System;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt
{
	public class T4ActionAdapterInterrupter : IT4CodeGenerationInterrupter
	{
		[NotNull]
		private Action<T4FailureRawData> Interrupter { get; }

		public T4ActionAdapterInterrupter([NotNull] Action<T4FailureRawData> interrupter) => Interrupter = interrupter;
		public void InterruptAfterProblem(T4FailureRawData failureData) => Interrupter(failureData);
	}
}
