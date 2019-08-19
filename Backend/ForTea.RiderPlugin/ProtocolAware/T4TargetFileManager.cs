using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.Documents;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	[SolutionComponent]
	public sealed class T4TargetFileManager : TemplateProcessing.Managing.Impl.T4TargetFileManager
	{
		[NotNull]
		private DocumentHost Host { get; }

		public T4TargetFileManager(
			[NotNull] ISolution solution,
			[NotNull] DocumentHost host
		) : base(solution) => Host = host;

		protected override void SyncDocuments(FileSystemPath destinationLocation) =>
			Host.SyncDocumentsWithFiles(destinationLocation);

		protected override void RefreshFiles(FileSystemPath destinationLocation)
		{
			var protocolSolution = Solution.GetProtocolSolution();
			protocolSolution
				.Editors
				.SaveFiles
				.Start(new List<string> {destinationLocation.FullPath});
			protocolSolution
				.GetFileSystemModel()
				.RefreshPaths
				.Start(new RdRefreshRequest(new List<string> {destinationLocation.FullPath}, true));
		}
	}
}
