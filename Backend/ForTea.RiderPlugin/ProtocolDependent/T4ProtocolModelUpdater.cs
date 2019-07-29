using GammaJul.ForTea.Core.ProtocolDependent;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolDependent
{
	[SolutionComponent]
	public class T4ProtocolModelUpdater : IT4ProtocolModelUpdater
	{
		[NotNull]
		private T4ProtocolModel Model { get; }

		[NotNull]
		private IT4TargetFileManager Manager { get; }

		public T4ProtocolModelUpdater([NotNull] ISolution solution, [NotNull] IT4TargetFileManager manager)
		{
			Manager = manager;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
		}

		public void UpdateFileInfo(IT4File file) =>
			Model.Configurations[file.GetSourceFile().GetLocation().FullPath.Replace("\\", "/")] =
				new T4ConfigurationModel(
					Manager.GetTemporaryExecutableLocation(file).FullPath.Replace("\\", "/"),
					Manager.GetTargetFileName(file)
				);
	}
}
