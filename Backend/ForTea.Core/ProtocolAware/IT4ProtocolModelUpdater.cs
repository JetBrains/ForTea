using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.ProtocolAware
{
	public interface IT4ProtocolModelUpdater
	{
		void UpdateFileInfo([NotNull] IT4File file);
	}
}
