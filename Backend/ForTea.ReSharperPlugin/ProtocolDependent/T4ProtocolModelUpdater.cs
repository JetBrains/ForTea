using GammaJul.ForTea.Core.ProtocolDependent;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.ReSharperPlugin.ProtocolDependent
{
	[SolutionComponent]
	public class T4ProtocolModelUpdater : IT4ProtocolModelUpdater
	{
		public void UpdateFileInfo(IT4File file)
		{
			// no protocol in R#
		}
	}
}
