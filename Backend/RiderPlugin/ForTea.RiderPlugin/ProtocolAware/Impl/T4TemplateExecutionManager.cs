using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.ProjectsHost.SolutionHost.Progress;
using JetBrains.RdBackend.Common.Features.ProjectModel;
using JetBrains.RdBackend.Common.Features.ProjectModel.View;
using JetBrains.ReSharper.Feature.Services.Protocol;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model.Notifications;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
  [SolutionComponent]
  public sealed class T4TemplateExecutionManager : IT4TemplateExecutionManager
  {
    [NotNull] private IDictionary<VirtualFileSystemPath, T4EnvDTEHost> RunningFiles { get; }

    [NotNull] private object ExecutionLocker { get; } = new object();

    [NotNull] private T4ProtocolModel Model { get; }

    private Lifetime Lifetime { get; }

    [NotNull] private ISolution Solution { get; }

    [NotNull] private IT4ProjectModelTemplateMetadataManager TemplateMetadataManager { get; }

    [NotNull] private BackgroundProgressManager BackgroundProgressManager { get; }

    [NotNull] private ProjectModelViewHost ProjectModelViewHost { get; }

    [NotNull] private NotificationsModel NotificationsModel { get; }

    [NotNull] private ILogger Logger { get; }

    public T4TemplateExecutionManager(
      Lifetime lifetime,
      [NotNull] ISolution solution,
      [NotNull] ProjectModelViewHost projectModelViewHost,
      [NotNull] NotificationsModel notificationsModel,
      [NotNull] ILogger logger,
      [NotNull] IT4ProjectModelTemplateMetadataManager templateMetadataManager,
      [NotNull] BackgroundProgressManager backgroundProgressManager
    )
    {
      Lifetime = lifetime;
      Solution = solution;
      ProjectModelViewHost = projectModelViewHost;
      NotificationsModel = notificationsModel;
      Logger = logger;
      TemplateMetadataManager = templateMetadataManager;
      BackgroundProgressManager = backgroundProgressManager;
      Model = solution.GetProtocolSolution().GetT4ProtocolModel();
      RunningFiles = new Dictionary<VirtualFileSystemPath, T4EnvDTEHost>();
    }

    public void RememberExecution(IT4File file, bool withProgress) =>
      RememberExecution(file.PhysicalPsiSourceFile.NotNull().GetLocation(), withProgress);

    public int GetEnvDTEPort(IT4File file)
    {
      bool hasInfo = RunningFiles.TryGetValue(file.PhysicalPsiSourceFile.GetLocation(), out var info);
      if (!hasInfo) return 0;
      return info.ConnectionManager.Port;
    }

    private void RememberExecution([NotNull] VirtualFileSystemPath path, bool withProgress)
    {
      var definition = Lifetime.CreateNested();
      if (withProgress)
      {
        var progress = new ProgressIndicator(definition.Lifetime);
        IProgressIndicator iProgress = progress;
        iProgress.Start(1);
        progress.Advance();
        var task = BackgroundProgressBuilder
          .FromProgressIndicator(progress)
          .AsIndeterminate()
          .WithHeader("Executing template")
          .WithDescription($"{path.Name}")
          .Build();
        Solution.Locks.ExecuteOrQueueEx(
          definition.Lifetime,
          "T4 execution progress launching",
          () => BackgroundProgressManager.AddNewTask(definition.Lifetime, task)
        );
      }

      RunningFiles[path] = new T4EnvDTEHost(definition, Solution);
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

        RememberExecution(file, false);
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

        RememberExecution(file, true);
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

        RememberExecution(file, false);
      }

      Model.RequestDebug.Start(new T4ExecutionRequest(GetT4FileLocation(file), true));
    }

    private bool IsExecutionRunning([NotNull] IT4File file) =>
      IsExecutionRunning(file.PhysicalPsiSourceFile.NotNull());

    public bool IsExecutionRunning(IPsiSourceFile file) =>
      RunningFiles.ContainsKey(file.GetLocation());

    public void OnExecutionFinished(IT4File file)
    {
      lock (ExecutionLocker)
      {
        var location = file.PhysicalPsiSourceFile.GetLocation();
        var runtime = RunningFiles[location];
        runtime.LifetimeDefinition.Terminate();
        RunningFiles.Remove(location);
      }
    }

    [NotNull]
    private T4FileLocation GetT4FileLocation([NotNull] IT4File file)
    {
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      var projectFile = sourceFile.ToProjectFile().NotNull();
      int id = ProjectModelViewHost.GetIdByItem(projectFile);
      return new T4FileLocation(id);
    }

    private void ShowNotification() => NotificationsModel.Notification(new NotificationModel(
      "Could not execute T4 file",
      "Execution is already running",
      true,
      RdNotificationEntryType.ERROR,
      new List<NotificationHyperlink>()
    ));
  }
}