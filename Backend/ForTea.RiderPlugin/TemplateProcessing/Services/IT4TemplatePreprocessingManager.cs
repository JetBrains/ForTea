using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services
{
	public interface IT4TemplatePreprocessingManager
	{
		void Preprocess([NotNull] IT4File file);
	}
}
