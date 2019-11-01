using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model;
using JetBrains.Rider.Model.Notifications;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
	[SolutionComponent]
	public sealed class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull]
		private ISet<FileSystemPath> RunningFiles { get; }

		[NotNull]
		private object ExecutionLocker { get; } = new object();

		[NotNull]
		private T4ProtocolModel Model { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IT4ProjectModelTemplateMetadataManager TemplateMetadataManager { get; }

		[NotNull]
		private ProjectModelViewHost ProjectModelViewHost { get; }

		[NotNull]
		private NotificationsModel NotificationsModel { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4TemplateExecutionManager(
			[NotNull] ISolution solution,
			[NotNull] ProjectModelViewHost projectModelViewHost,
			[NotNull] NotificationsModel notificationsModel,
			[NotNull] ILogger logger,
			[NotNull] IT4ProjectModelTemplateMetadataManager templateMetadataManager
		)
		{
			Solution = solution;
			ProjectModelViewHost = projectModelViewHost;
			NotificationsModel = notificationsModel;
			Logger = logger;
			TemplateMetadataManager = templateMetadataManager;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RunningFiles = new HashSet<FileSystemPath>();
		}

		public void UpdateTemplateKind(IT4File file)
		{
			Logger.Verbose("Updating template kind");
			var projectFile = file.PhysicalPsiSourceFile.ToProjectFile().NotNull();
			Solution.InvokeUnderTransaction(cookie =>
				TemplateMetadataManager.UpdateTemplateMetadata(cookie, projectFile, T4TemplateKind.Executable));
			// Apply the changes, just in case the template kind was different
			Solution.GetPsiServices().Files.CommitAllDocuments();
		}

		public void Execute(IT4File file)
		{
			Logger.Verbose("Trying to execute a file");
			lock (ExecutionLocker)
			{
				if (IsExecutionRunning(file))
				{
					ShowNotification();
					return;
				}

				RunningFiles.Add(file.GetSourceFile().GetLocation());
			}

			Model.RequestExecution.Start(new T4ExecutionRequest(GetT4FileLocation(file), true));
		}

		public void ExecuteSilently(IT4File file)
		{
			Logger.Verbose("Trying to execute a file silently");
			lock (ExecutionLocker)
			{
				if (IsExecutionRunning(file))
				{
					ShowNotification();
					return;
				}

				RunningFiles.Add(file.GetSourceFile().GetLocation());
			}

			Model.RequestExecution.Start(new T4ExecutionRequest(GetT4FileLocation(file), false));
		}

		public void Debug(IT4File file)
		{
			Logger.Verbose("Trying to debug a file");
			lock (ExecutionLocker)
			{
				if (IsExecutionRunning(file))
				{
					ShowNotification();
					return;
				}

				RunningFiles.Add(file.GetSourceFile().GetLocation());
			}

			Model.RequestDebug.Start(new T4ExecutionRequest(GetT4FileLocation(file), true));
		}

		private bool IsExecutionRunning([NotNull] IT4File file) => IsExecutionRunning(file.GetSourceFile().NotNull());
		public bool IsExecutionRunning(IPsiSourceFile file) => RunningFiles.Contains(file.GetLocation());

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

		private void ShowNotification()
		{
			var notification = new NotificationModel(
				"Could not execute T4 file",
				"Execution is already running", true,
				RdNotificationEntryType.ERROR
			);
			NotificationsModel.Notification(notification);
		}
	}
}
