using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4ExecutionResult
	{
		void Save([NotNull] FileSystemPath destination);
	}
}
