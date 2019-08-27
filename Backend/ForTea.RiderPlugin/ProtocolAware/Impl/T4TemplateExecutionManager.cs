using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
	[SolutionComponent]
	public class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private ISet<FileSystemPath> RunningFiles { get; }

		[NotNull]
		private object ExecutionLocker { get; } = new object();

		[NotNull]
		private T4ProtocolModel Model { get; }

		[NotNull]
		private ProjectModelViewHost ProjectModelViewHost { get; }

		public T4TemplateExecutionManager(
			[NotNull] ISolution solution,
			[NotNull] ProjectModelViewHost projectModelViewHost,
			[NotNull] ILogger logger
		)
		{
			ProjectModelViewHost = projectModelViewHost;
			Logger = logger;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RunningFiles = new HashSet<FileSystemPath>();
		}

		public void Execute(IT4File file)
		{
			lock (ExecutionLocker)
			{
				if (IsExecutionRunning(file))
				{
					Logger.Warn("Could not execute template: execution already running");
					return;
				}

				RunningFiles.Add(file.GetSourceFile().GetLocation());
			}

			Model.RequestExecution.Start(GetT4FileLocation(file));
		}

		public void Debug(IT4File file)
		{
			lock (ExecutionLocker)
			{
				if (IsExecutionRunning(file))
				{
					Logger.Warn("Could not execute template: execution already running");
					return;
				}

				RunningFiles.Add(file.GetSourceFile().GetLocation());
			}

			Model.RequestDebug.Start(GetT4FileLocation(file));
		}

		public bool IsExecutionRunning(IT4File file) => RunningFiles.Contains(file.GetSourceFile().GetLocation());

		public void OnExecutionFinished(IT4File file)
		{
			lock (ExecutionLocker)
			{
				RunningFiles.Remove(file.GetSourceFile().GetLocation());
			}
		}

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
