using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName
{
	public interface IT4GeneratedNameProvider
	{
		[NotNull]
		string GeneratedClassName { get; }

		[NotNull]
		string GeneratedBaseClassName { get; }

		[NotNull]
		string GeneratedBaseClassFQN { get; }

		[NotNull]
		string TransformTextMethodName { get; }
	}
}
