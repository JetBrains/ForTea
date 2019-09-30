using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.PreprocessFromContext", "Preprocess Template")]
	public sealed class T4PreprocessTemplateAction : T4FileBasedActionBase
	{
		public override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
			var solution = FindSolution(context).NotNull();
			var model = solution.GetProtocolSolution().GetT4ProtocolModel();
			var targetFileManager = solution.GetComponent<IT4TargetFileManager>();
			var templateDataManager = solution.GetComponent<IT4ProjectModelTemplateDataManager>();
			var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
			var converter = solution.GetComponent<IT4BuildMessageConverter>();

			model.PreprocessingStarted();

			var file = FindT4File(context, solution).NotNull();
			var projectFile = file.GetSourceFile().ToProjectFile().NotNull();
			var location = new T4FileLocation(solution.GetComponent<ProjectModelViewHost>().GetIdByItem(projectFile));

			statistics.TrackAction("T4.Template.Preprocess");
			templateDataManager.SetTemplateKind(projectFile, T4TemplateKind.Preprocessed);
			try
			{
				string message = new T4CSharpCodeGenerator(file, solution).Generate().RawText;
				using (WriteLockCookie.Create())
				{
					targetFileManager.SavePreprocessResults(file, message);
				}

				model.PreprocessingFinished(new T4PreprocessingResult(location, true, null));
			}
			catch (T4OutputGenerationException e)
			{
				var message = converter.ToT4BuildMessage(e.FailureData);
				var result = new T4PreprocessingResult(location, false, message);
				model.PreprocessingFinished(result);
			}
		}
	}
}
