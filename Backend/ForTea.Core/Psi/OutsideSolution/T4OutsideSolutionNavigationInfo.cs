using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
	public sealed class T4OutsideSolutionNavigationInfo
	{
		[NotNull]
		public FileSystemPath FileSystemPath { get; }
		public DocumentRange DocumentRange { get; }

		public T4OutsideSolutionNavigationInfo(
			[NotNull] FileSystemPath fileSystemPath,
			DocumentRange documentRange
		)
		{
			FileSystemPath = fileSystemPath;
			DocumentRange = documentRange;
		}
	}
}
