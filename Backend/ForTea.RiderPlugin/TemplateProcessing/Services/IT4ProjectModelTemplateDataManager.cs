using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services
{
	public interface IT4ProjectModelTemplateDataManager
	{
		void SetTemplateKind([NotNull] IProjectFile file, T4TemplateKind kind);

		[CanBeNull]
		IProjectFile FindLastGenOutput([NotNull] IProjectFile file);

		void SetLastGenOutput([NotNull] IProjectFile source, [NotNull] IProjectFile generated);
	}
}
