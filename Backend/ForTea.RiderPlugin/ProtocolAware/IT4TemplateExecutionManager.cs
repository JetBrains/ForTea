using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Lifetimes;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	public interface IT4TemplateExecutionManager
	{
		void Execute(Lifetime lifetime, [NotNull] IT4File file);
		void Debug(Lifetime lifetime, [NotNull] IT4File file);
		bool CanExecute(IT4File file);
	}
}
