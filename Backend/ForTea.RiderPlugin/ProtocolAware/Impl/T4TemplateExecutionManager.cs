using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
	[SolutionComponent]
	public class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull]
		private T4ProtocolModel Model { get; }

		[NotNull]
		private ISolutionProcessStartInfoPatcher Patcher { get; }

		[NotNull]
		private IT4TargetFileManager TargetManager { get; }

		[NotNull]
		private ProjectModelViewHost ProjectModelViewHost { get; }

		public T4TemplateExecutionManager(
			[NotNull] ISolutionProcessStartInfoPatcher patcher,
			[NotNull] IT4TargetFileManager targetManager,
			[NotNull] ISolution solution,
			[NotNull] ProjectModelViewHost projectModelViewHost
		)
		{
			Patcher = patcher;
			TargetManager = targetManager;
			ProjectModelViewHost = projectModelViewHost;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
		}

		public void Execute(Lifetime lifetime, IT4File file) => Model.RequestExecution.Start(GetT4FileLocation(file));
		public void Debug(Lifetime lifetime, IT4File file) => Model.RequestDebug.Start(GetT4FileLocation(file));
		public bool CanExecute(IT4File file) => true; // TODO: check whether execution is running

		[NotNull]
		private T4FileLocation GetT4FileLocation([NotNull] IT4File file)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			int id = ProjectModelViewHost.GetIdByItem(projectFile);
			return new T4FileLocation(id);
		}
	}
}
