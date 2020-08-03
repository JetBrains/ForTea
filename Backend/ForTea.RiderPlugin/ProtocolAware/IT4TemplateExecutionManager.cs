using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	public interface IT4TemplateExecutionManager
	{
		void RememberExecution([NotNull] IT4File file, bool withProgress);
		int GetEnvDTEPort([NotNull] IT4File file);
		void UpdateTemplateKind([NotNull] IT4File file);
		void Execute([NotNull] IT4File file);
		void ExecuteSilently([NotNull] IT4File file);
		void Debug([NotNull] IT4File file);
		bool IsExecutionRunning([NotNull] IPsiSourceFile file);
		void OnExecutionFinished([NotNull] IT4File file);
	}
}
