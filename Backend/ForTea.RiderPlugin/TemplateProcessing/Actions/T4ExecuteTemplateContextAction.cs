using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4ExecuteTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string Message = "Execute T4 design-time template";

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute",
			Justification = "If base decides that action is available, File is guaranteed to be not nul")]
		public override bool IsAvailable(IUserDataHolder cache)
		{
			var manager = Provider.Solution.GetComponent<IT4TemplateExecutionManager>();
			return base.IsAvailable(cache) && manager.CanCompile(File);
		}

		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider)
		{
		}

		protected override void DoExecute(ISolution solution, IProgressIndicator progress)
		{
			LaunchProgress(progress);
			var manager = solution.GetComponent<IT4TemplateExecutionManager>();
			Assertion.AssertNotNull(File, "File != null");
			Assertion.Assert(manager.CanCompile(File), "manager.CanCompile(File)");
			if (!manager.Compile(solution.GetLifetime(), File)) return;
			var result = manager.Execute(solution.GetLifetime(), File, progress);
			var fileManager = solution.GetComponent<IT4TargetFileManager>();
			using (WriteLockCookie.Create())
			{
				fileManager.SaveResults(result, File);
			}
		}

		private static void LaunchProgress([CanBeNull] IProgressIndicator indicator)
		{
			if (indicator == null) return;
			indicator.Start(1);
			indicator.Advance();
			indicator.TaskName = "Executing T4 Template";
			indicator.CurrentItemText = "Preparing";
		}

		public override string Text => Message;
	}
}
