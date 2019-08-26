using System.Diagnostics;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.Application.Progress;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull]
		private ISolutionProcessStartInfoPatcher Patcher { get; }

		[NotNull]
		private IT4TargetFileManager TargetManager { get; }

		public T4TemplateExecutionManager(
			[NotNull] ISolutionProcessStartInfoPatcher patcher,
			[NotNull] IT4TargetFileManager targetManager
		)
		{
			Patcher = patcher;
			TargetManager = targetManager;
		}

		public bool Execute(Lifetime lifetime, IT4File file, IProgressIndicator progress = null)
		{
			var executablePath = TargetManager.GetTemporaryExecutableLocation(file);
			string targetFileName = TargetManager.GetExpectedTargetFileName(file);
			var destinationPath = executablePath.Directory.Combine(targetFileName);
			var process = LaunchProcess(lifetime, executablePath, destinationPath);
			lifetime.ThrowIfNotAlive();
			int code = process.WaitForExitSpinning(100, progress);
			lifetime.ThrowIfNotAlive();
			return code == 0;
		}

		private Process LaunchProcess(
			Lifetime lifetime,
			[NotNull] FileSystemPath executablePath,
			[NotNull] FileSystemPath destinationPath
		)
		{
			var startInfo = new JetProcessStartInfo(new ProcessStartInfo
			{
				UseShellExecute = false,
				FileName = executablePath.FullPath,
				CreateNoWindow = true,
				Arguments = destinationPath.FullPath
			});

			var request = JetProcessRuntimeRequest.CreateFramework();
			var patchedInfo = Patcher.Patch(startInfo, request).GetPatchedInfoOrThrow();
			var process = new Process
			{
				StartInfo = patchedInfo.ToProcessStartInfo()
			};
			lifetime.OnTermination(process);
			lifetime.Bracket(
				() => process.Start(),
				() => Logger.CatchSilent(() =>
				{
					if (!process.HasExited) process.KillTree();
				})
			);
			return process;
		}
	}
}
