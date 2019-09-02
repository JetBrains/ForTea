using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public interface IT4ProjectModelTemplateDataManager
	{
		T4TemplateKind GetTemplateKind([NotNull] IProjectFile file);
		void SetTemplateKind([NotNull] IProjectFile file, T4TemplateKind kind);

		[CanBeNull]
		IProjectFile FindLastGenOutput([NotNull] IProjectFile file);

		void SetLastGenOutput([NotNull] IProjectFile source, [NotNull] IProjectFile generated);
	}
}
