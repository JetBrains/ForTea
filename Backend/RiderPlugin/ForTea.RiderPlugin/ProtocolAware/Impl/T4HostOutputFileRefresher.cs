using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl;
using JetBrains.IDE.PerClientComponents;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features;
using JetBrains.RdBackend.Common.Features.Documents;
using JetBrains.Rider.Backend.Features.Documents;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
	[SolutionPerClientComponent]
	public sealed class T4HostOutputFileRefresher : T4BasicOutputFileRefresher
	{
		public T4HostOutputFileRefresher(ISolution solution) : base(solution)
		{
		}

		public override void Refresh(IProjectFile output)
		{
			base.Refresh(output);
			SyncDocuments(output.Location);
			RefreshFiles(output.Location);
		}

		private void SyncDocuments([NotNull] VirtualFileSystemPath destinationLocation) =>
			Solution.SyncDocumentsWithFiles(destinationLocation);

		private void RefreshFiles([NotNull] VirtualFileSystemPath destinationLocation) => Solution
			.GetProtocolSolution()
			.GetFileSystemModel()
			.RefreshPaths
			.Start(new RdFsRefreshRequest(new List<string> {destinationLocation.FullPath}, true));
	}
}
