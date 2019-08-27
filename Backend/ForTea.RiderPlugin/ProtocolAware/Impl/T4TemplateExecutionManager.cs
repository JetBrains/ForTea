using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
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
		private ProjectModelViewHost ProjectModelViewHost { get; }

		public T4TemplateExecutionManager(
			[NotNull] ISolution solution,
			[NotNull] ProjectModelViewHost projectModelViewHost
		)
		{
			ProjectModelViewHost = projectModelViewHost;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
		}

		public void Execute(IT4File file) => Model.RequestExecution.Start(GetT4FileLocation(file));
		public void Debug(IT4File file) => Model.RequestDebug.Start(GetT4FileLocation(file));
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
