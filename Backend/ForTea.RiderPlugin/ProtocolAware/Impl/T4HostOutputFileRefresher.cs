using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.Documents;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
	[SolutionComponent]
	public sealed class T4HostOutputFileRefresher : T4BasicOutputFileRefresher
	{
		[NotNull]
		private DocumentHost Host { get; }

		public T4HostOutputFileRefresher(ISolution solution, [NotNull] DocumentHost host) : base(solution) =>
			Host = host;

		public override void Refresh(IProjectFile output)
		{
			base.Refresh(output);
			SyncDocuments(output.Location);
			RefreshFiles(output.Location);
		}

		private void SyncDocuments([NotNull] FileSystemPath destinationLocation) =>
			Host.SyncDocumentsWithFiles(destinationLocation);

		private void RefreshFiles([NotNull] FileSystemPath destinationLocation) => Solution
			.GetProtocolSolution()
			.GetFileSystemModel()
			.RefreshPaths
			.Start(new RdRefreshRequest(new List<string> {destinationLocation.FullPath}, true));
	}
}
