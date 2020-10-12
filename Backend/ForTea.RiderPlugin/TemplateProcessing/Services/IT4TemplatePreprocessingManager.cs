using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.Model;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services
{
	public interface IT4TemplatePreprocessingManager
	{
		void TryPreprocess([NotNull] IT4File file);

		[NotNull]
		T4PreprocessingResult Preprocess([NotNull] IT4File file);
	}
}
