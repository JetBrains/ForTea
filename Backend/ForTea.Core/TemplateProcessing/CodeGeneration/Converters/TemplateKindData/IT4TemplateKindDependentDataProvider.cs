using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.TemplateKindData
{
	public interface IT4TemplateKindDependentDataProvider
	{
		[NotNull]
		string GeneratedClassName { get; }

		[NotNull]
		string GeneratedBaseClassName { get; }

		[NotNull]
		string GeneratedBaseClassFQN { get; }

		[NotNull]
		string TransformTextMethodName { get; }

		[NotNull]
		string TransformTextAttributes { get; }
	}
}
