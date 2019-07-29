using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.ProtocolAware
{
	public interface IT4ProtocolModelManager
	{
		void UpdateFileInfo([NotNull] IT4File file);
	}
}
