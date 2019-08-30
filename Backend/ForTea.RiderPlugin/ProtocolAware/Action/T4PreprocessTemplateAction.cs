using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model.Notifications;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.PreprocessFromContext", "Preprocess Template")]
	public sealed class T4PreprocessTemplateAction : T4FileBasedActionBase
	{
		public override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
			var solution = FindSolution(context).NotNull();
			var file = FindT4File(context).NotNull();
			var targetFileManager = solution.GetComponent<IT4TargetFileManager>();
			var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
			statistics.TrackAction("T4.Template.Preprocess");
			try
			{
				string message = new T4CSharpCodeGenerator(file, solution).Generate().RawText;
				using (WriteLockCookie.Create())
				{
					targetFileManager.SavePreprocessResults(file, message);
				}
			}
			catch (T4OutputGenerationException e)
			{
				// TODO: show as build output?
				var notificationModel = new NotificationModel(
					"Could not preprocess template",
					$"File contains syntax errors:\n{e.FailureData.Message}",
					true,
					RdNotificationEntryType.ERROR);
				solution.GetComponent<NotificationsModel>().Notification(notificationModel);
			}
		}
	}
}
