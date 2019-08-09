using System;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt
{
	public class T4OutputGenerationException : Exception
	{
		public T4FailureRawData FailureData { get; }
		public T4OutputGenerationException(T4FailureRawData failureData) => FailureData = failureData;
	}
}
