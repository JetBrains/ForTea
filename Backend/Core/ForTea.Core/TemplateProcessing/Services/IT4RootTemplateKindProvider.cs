using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public interface IT4RootTemplateKindProvider
	{
		T4TemplateKind GetRootTemplateKind([NotNull] IPsiSourceFile file);
	}
}
