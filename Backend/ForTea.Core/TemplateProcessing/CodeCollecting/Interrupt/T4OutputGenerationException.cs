using System;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt
{
	public sealed class T4OutputGenerationException : Exception
	{
		public FrugalLocalList<T4FailureRawData> FailureDatum { get; }

		public T4OutputGenerationException(T4FailureRawData failureData) =>
			FailureDatum = FrugalLocalList<T4FailureRawData>.Init(failureData);

		public T4OutputGenerationException(FrugalLocalList<T4FailureRawData> failureDatum) =>
			FailureDatum = failureDatum;
	}
}
