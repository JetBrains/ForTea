using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TemplateExecutionManager
	{
		IT4ExecutionResult Execute([NotNull] IT4File file, [CanBeNull] IProgressIndicator progress = null);
	}
}
