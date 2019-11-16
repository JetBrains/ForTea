using System;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators
{
	public abstract class T4CSharpCodeGeneratorBase
	{
		private const string DefaultErrorMessage = "ErrorGeneratingOutput";

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpCodeGeneratorBase([NotNull] IT4File file) => File = file;

		[NotNull]
		public T4CSharpCodeGenerationResult Generate() =>
			CreateConverter(Collector.Collect()).Convert();

		[NotNull]
		public T4CSharpCodeGenerationResult GenerateSafe()
		{
			try
			{
				return Generate();
			}
			catch (T4OutputGenerationException)
			{
				var result = new T4CSharpCodeGenerationResult(File);
				result.Append(DefaultErrorMessage);
				return result;
			}
		}

		[NotNull]
		protected abstract T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		[NotNull]
		protected abstract T4CSharpIntermediateConverterBase CreateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult);
	}
}
