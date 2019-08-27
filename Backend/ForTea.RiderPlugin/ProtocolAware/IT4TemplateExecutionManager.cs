using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	public interface IT4TemplateExecutionManager
	{
		void Execute([NotNull] IT4File file);
		void Debug([NotNull] IT4File file);
		bool CanExecute(IT4File file);
	}
}
