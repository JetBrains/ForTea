using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Utils
{
	public interface IT4IncludeGuard<in T>
	{
		void StartProcessing([NotNull] T file);
		void EndProcessing();
		void TryEndProcessing([CanBeNull] T file); // TODO: remove
		bool CanProcess([NotNull] T file);
		bool HasSeenFile([NotNull] T file);
		bool IsOnTopLevel { get; }
	}
}