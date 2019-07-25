using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.RunConfiguration;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4DebugTemplateContextAction : T4FileBasedContextActionBase
	{
		private const string Message = "Debug Template";

		public override string Text => Message;

		public T4DebugTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider provider) :
			base(provider)
		{
		}

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute",
			Justification = "If base decides that action is available, File is guaranteed to be not nul")]
		public override bool IsAvailable(IUserDataHolder cache)
		{
			var manager = Provider.Solution.GetComponent<IT4TemplateExecutionManager>();
			return base.IsAvailable(cache) && manager.CanCompile(File);
		}
		
		protected override void DoExecute(ISolution solution, IProgressIndicator progress)
		{
			var manager = solution.GetComponent<IT4TemplateExecutionManager>();
			Assertion.AssertNotNull(File, "File != null");
			Assertion.Assert(manager.CanCompile(File), "manager.CanCompile(File)");
			if (!manager.Compile(solution.GetLifetime(), File)) return;
			var host = solution.GetComponent<RunConfigurationHost>();

			int x = 2 + 2;
//			host.CreateAndRun(TODO);
		}
	}
}
