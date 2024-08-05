using System.IO;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
  [SolutionComponent(InstantiationEx.LegacyDefault)]
  public sealed class T4TargetFileManager : IT4TargetFileManager
  {
    [NotNull] private ISolution Solution { get; }

    [NotNull] private IShellLocks Locks { get; }

    [NotNull] private IT4ProjectModelTemplateMetadataManager TemplateMetadataManager { get; }

    [NotNull] private IT4OutputFileRefresher OutputFileRefresher { get; }

    public T4TargetFileManager(
      [NotNull] ISolution solution,
      [NotNull] IShellLocks locks,
      [NotNull] IT4ProjectModelTemplateMetadataManager templateMetadataManager,
      [NotNull] IT4OutputFileRefresher outputFileRefresher
    )
    {
      Solution = solution;
      Locks = locks;
      TemplateMetadataManager = templateMetadataManager;
      OutputFileRefresher = outputFileRefresher;
    }

    public VirtualFileSystemPath GetTemporaryExecutableLocation(IT4File file)
    {
      var projectFile = file.PhysicalPsiSourceFile.ToProjectFile();
      var project = projectFile?.GetProject();
      if (project == null) return VirtualFileSystemPath.GetEmptyPathFor(InteractionContext.SolutionContext);
      var relativePath = projectFile.Location.MakeRelativeTo(project.Location.Parent);
      var ttLocation = VirtualFileSystemDefinition.GetTempPath(InteractionContext.SolutionContext)
        .Combine("TextTemplating")
        .Combine(relativePath);
      return ttLocation.Parent.Combine(ttLocation.Name.WithoutExtension()).Combine("GeneratedTransformation.exe");
    }

    public VirtualFileSystemPath GetExpectedTemporaryTargetFileLocation(IT4File file) =>
      GetTemporaryExecutableLocation(file).Parent.Combine(GetExpectedTargetFileName(file));

    [NotNull]
    private string GetExpectedTargetFileName([NotNull] IT4File file)
    {
      Locks.AssertReadAccessAllowed();
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      string name = sourceFile.Name;
      string targetExtension = file.GetTargetExtension() ?? T4CSharpCodeGenerationUtils.DefaultTargetExtension;
      return name.WithOtherExtension(targetExtension);
    }

    [NotNull]
    private VirtualFileSystemPath GetTemporaryTargetFileFolder([NotNull] IT4File file) =>
      GetTemporaryExecutableLocation(file).Parent;

    [CanBeNull]
    private VirtualFileSystemPath TryFindTemporaryTargetFile([NotNull] IT4File file)
    {
      string name = file.PhysicalPsiSourceFile.NotNull().Name.WithOtherExtension(".*");
      var candidates = GetTemporaryTargetFileFolder(file).GetChildFiles(name);
      return candidates.FirstOrDefault();
    }

    [NotNull]
    private string GetPreprocessingTargetFileName([NotNull] IT4File file)
    {
      Locks.AssertReadAccessAllowed();
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      string name = sourceFile.Name;
      return name.WithOtherExtension("cs");
    }

    [NotNull]
    private IProjectFile GetOrCreateSameDestinationFile(
      [NotNull] IProjectModelTransactionCookie cookie,
      [NotNull] IT4File file,
      [NotNull] VirtualFileSystemPath temporary
    ) => GetOrCreateSameDestinationFile(cookie, file, temporary.Name);

    [NotNull]
    private IProjectFile GetOrCreateSameDestinationFile(
      [NotNull] IProjectModelTransactionCookie cookie,
      [NotNull] IT4File file,
      [NotNull] string destinationName
    )
    {
      Locks.AssertWriteAccessAllowed();
      var existingFile = GetSameDestinationFile(file, destinationName);
      if (existingFile != null) return existingFile;
      return CreateSameDestinationFile(cookie, file, destinationName);
    }

    [NotNull]
    private IProjectFile CreateSameDestinationFile(
      [NotNull] IProjectModelTransactionCookie cookie,
      [NotNull] IT4File file,
      [NotNull] string destinationName
    )
    {
      Locks.AssertWriteAccessAllowed();
      var projectFile = file.PhysicalPsiSourceFile.ToProjectFile().NotNull();
      var folder = projectFile.ParentFolder.NotNull();
      var targetLocation = folder.Location.Combine(destinationName);
      var parameters = T4MSBuildProjectUtil.CreateTemplateMetadata(projectFile);
      return cookie.AddFile(folder, targetLocation, parameters);
    }

    [CanBeNull]
    private IProjectFile GetSameDestinationFile([NotNull] IT4File file, [NotNull] string temporaryName)
    {
      Locks.AssertWriteAccessAllowed();
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      var candidates = sourceFile
        .ToProjectFile()
        ?.ParentFolder
        ?.GetSubItems(temporaryName)
        .ToList()
        .OfType<IProjectFile>()
        .AsList();
      Assertion.AssertNotNull(candidates, "candidates != null");
      Assertion.Assert(candidates.Count <= 1, "candidates.Value.Length <= 1");
      return candidates.SingleOrDefault();
    }

    [NotNull]
    private VirtualFileSystemPath GetDestinationLocation([NotNull] IT4File file, [NotNull] string temporaryName)
    {
      Locks.AssertReadAccessAllowed();
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      return sourceFile.ToProjectFile().NotNull().Location.Parent.Combine(temporaryName);
    }

    public void TryProcessExecutionResults(IT4File file)
    {
      Locks.AssertReadAccessAllowed();
      Locks.AssertWriteAccessAllowed();
      var temporary = TryFindTemporaryTargetFile(file);
      if (temporary == null) return;
      var destinationLocation = GetDestinationLocation(file, temporary.Name);
      destinationLocation.DeleteFile();
      File.Move(temporary.FullPath, destinationLocation.FullPath);
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      var projectFile = sourceFile.ToProjectFile().NotNull();
      IProjectFile destination = null;
      Solution.InvokeUnderTransaction(cookie =>
      {
        destination = UpdateProjectModel(file, destinationLocation, cookie);
        RemoveLastGenOutputIfDifferent(file, cookie, destinationLocation);
        TemplateMetadataManager.UpdateTemplateMetadata(
          cookie,
          projectFile,
          T4TemplateKind.Executable,
          destinationLocation
        );
        TemplateMetadataManager.UpdateGeneratedFileMetadata(cookie, destination, projectFile);
      });
      OutputFileRefresher.Refresh(destination);
    }

    private void RemoveLastGenOutputIfDifferent(
      [NotNull] IT4File file,
      [NotNull] IProjectModelTransactionCookie cookie,
      [NotNull] VirtualFileSystemPath destinationLocation
    )
    {
      var projectFile = file.PhysicalPsiSourceFile?.ToProjectFile();
      if (projectFile == null) return;
      foreach (var suspect in TemplateMetadataManager
                 .FindLastGenOutput(projectFile)
                 .Where(it => it.Location != destinationLocation)
              )
      {
        cookie.Remove(suspect);
      }
    }

    [NotNull]
    private IProjectFile UpdateProjectModel(
      [NotNull] IT4File file,
      [NotNull] VirtualFileSystemPath result,
      [NotNull] IProjectModelTransactionCookie cookie
    )
    {
      Locks.AssertReadAccessAllowed();
      Locks.AssertWriteAccessAllowed();
      var destination = GetOrCreateSameDestinationFile(cookie, file, result);
      return destination;
    }

    public void SavePreprocessResults(IT4File file, string text)
    {
      Locks.AssertReadAccessAllowed();
      var sourceFile = file.PhysicalPsiSourceFile.NotNull();
      var projectFile = sourceFile.ToProjectFile().NotNull();
      Locks.AssertWriteAccessAllowed();
      VirtualFileSystemPath destinationLocation = null;
      IProjectFile destination = null;
      Solution.InvokeUnderTransaction(cookie =>
      {
        string destinationName = GetPreprocessingTargetFileName(file);
        destination = GetOrCreateSameDestinationFile(cookie, file, destinationName);
        destinationLocation = destination.Location;
        RemoveLastGenOutputIfDifferent(file, cookie, destinationLocation);
        TemplateMetadataManager.UpdateTemplateMetadata(
          cookie,
          projectFile,
          T4TemplateKind.Preprocessed,
          destinationLocation
        );
        TemplateMetadataManager.UpdateGeneratedFileMetadata(cookie, destination, projectFile);
      });
      destinationLocation.WriteAllText(text);
      OutputFileRefresher.Refresh(destination);
    }
  }
}