using GammaJul.ForTea.Core.Tree;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.ProtocolAware.Impl
{
	[SolutionComponent]
	public class T4ProtocolModelUpdater : IT4ProtocolModelUpdater
	{
		public virtual void UpdateFileInfo(IT4File file)
		{
			// We are not in IRiderFeatureZone!
			// Must be R# or a test case
			// No luck, cannot access protocol
		}
	}
}
