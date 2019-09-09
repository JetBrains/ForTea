using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.Managing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Caches.SymbolCache;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	/// Note that this component is overridden in
	/// <see cref="JetBrains.ForTea.RiderPlugin.ProtocolAware.T4TargetFileManager">protocol</see> namespace 
	[SolutionComponent]
	public class T4TargetFileManager : IT4TargetFileManager
	{
		[NotNull]
		protected ISolution Solution { get; }

		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private IT4TargetFileChecker TargetFileChecker { get; }

		public T4TargetFileManager(
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileChecker targetFileChecker,
			[NotNull] IShellLocks locks
		)
		{
			Solution = solution;
			TargetFileChecker = targetFileChecker;
			Locks = locks;
		}

		public FileSystemPath GetTemporaryExecutableLocation(IT4File file)
		{
			var projectFile = file.GetSourceFile().ToProjectFile();
			var project = projectFile?.GetProject();
			if (project == null) return FileSystemPath.Empty;
			var relativePath = projectFile.Location.MakeRelativeTo(project.Location.Parent);
			var ttLocation = project.GetIntermediateDirectory(project.GetCurrentTargetFrameworkId())
				.Combine("TextTemplating")
				.Combine(relativePath);
			return ttLocation.Parent.Combine(ttLocation.Name.WithoutExtension()).Combine("GeneratedTransformation.exe");
		}

		public FileSystemPath GetExpectedTemporaryTargetFileLocation(IT4File file) =>
			GetTemporaryExecutableLocation(file).Parent.Combine(GetExpectedTargetFileName(file));

		[NotNull]
		private string GetExpectedTargetFileName([NotNull] IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			string name = sourceFile.Name;
			string targetExtension = file.GetTargetExtension();
			return name.WithOtherExtension(targetExtension);
		}

		[NotNull]
		private FileSystemPath GetTemporaryTargetFileFolder([NotNull] IT4File file) =>
			GetTemporaryExecutableLocation(file).Parent;

		[CanBeNull]
		private FileSystemPath TryFindTemporaryTargetFile([NotNull] IT4File file)
		{
			string name = file.GetSourceFile().NotNull().Name.WithOtherExtension(".*");
			var candidates = GetTemporaryTargetFileFolder(file).GetChildFiles(name);
			return candidates.FirstOrDefault();
		}

		[NotNull]
		private string GetPreprocessingTargetFileName([NotNull] IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			string name = sourceFile.Name;
			return name.WithOtherExtension("cs");
		}

		[NotNull]
		private IProjectFile GetOrCreateSameDestinationFile(
			[NotNull] IProjectModelTransactionCookie cookie,
			[NotNull] IT4File file,
			[NotNull] FileSystemPath temporary
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
			var projectFile = file.GetSourceFile().ToProjectFile().NotNull();
			var folder = projectFile.ParentFolder.NotNull();
			var targetLocation = folder.Location.Combine(destinationName);
			var parameters = T4MSBuildProjectUtil.CreateTemplateMetadata(projectFile);
			return cookie.AddFile(folder, targetLocation, parameters);
		}

		[CanBeNull]
		private IProjectFile GetSameDestinationFile([NotNull] IT4File file, [NotNull] string temporaryName)
		{
			Locks.AssertWriteAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			var candidates = sourceFile
				.ToProjectFile()
				?.ParentFolder
				?.GetSubItems(temporaryName)
				.AsEnumerable()
				.OfType<IProjectFile>()
				.AsList();
			Assertion.AssertNotNull(candidates, "candidates != null");
			Assertion.Assert(candidates.Count <= 1, "candidates.Value.Length <= 1");
			return candidates.SingleOrDefault();
		}

		[NotNull]
		private FileSystemPath GetDestinationLocation([NotNull] IT4File file, [NotNull] string temporaryName)
		{
			Locks.AssertReadAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			return sourceFile.ToProjectFile().NotNull().Location.Parent.Combine(temporaryName);
		}

		public void TryProcessExecutionResults(IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessAllowed();
			var temporary = TryFindTemporaryTargetFile(file);
			if (temporary == null) return;
			Solution.InvokeUnderTransaction(cookie =>
			{
				RemoveLastGenOutput(file, cookie);
				var destinationLocation = GetDestinationLocation(file, temporary.Name);
				temporary.MoveFile(destinationLocation, true);
				UpdateProjectModel(file, destinationLocation, cookie);
				UpdateLastGetOutput(file, destinationLocation, cookie);
			});
		}

		private void UpdateLastGetOutput(
			[NotNull] IT4File file,
			[NotNull] FileSystemPath destinationLocation,
			[NotNull] IProjectModelTransactionCookie cookie
		)
		{
			var projectFile = file.GetSourceFile()?.ToProjectFile();
			if (projectFile == null) return;
			cookie.EditFileProperties(projectFile, properties =>
			{
				if (!(properties is ProjectFileProperties projectFileProperties)) return;
				projectFileProperties.CustomToolOutput = destinationLocation.Name;
			});
		}

		private void RemoveLastGenOutput([NotNull] IT4File file, [NotNull] IProjectModelTransactionCookie cookie)
		{
			var projectFile = file.GetSourceFile()?.ToProjectFile();
			if (projectFile == null) return;
			if (!(projectFile.Properties is ProjectFileProperties properties)) return;
			string output = properties.CustomToolOutput;
			var folder = projectFile.ParentFolder;
			if (folder == null) return;
			var suspects = folder
				.GetSubItems(output)
				.AsEnumerable()
				.OfType<IProjectFile>()
				.Where(it => TargetFileChecker.IsGeneratedFrom(it, projectFile));
			foreach (var suspect in suspects)
			{
				cookie.Remove(suspect);
			}
		}

		private void UpdateProjectModel(
			[NotNull] IT4File file,
			[NotNull] FileSystemPath result,
			[NotNull] IProjectModelTransactionCookie cookie
		)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessAllowed();
			var destination = GetOrCreateSameDestinationFile(cookie, file, result);
			SyncDocuments(destination.Location);
			var sourceFile = destination.ToSourceFile();
			if (sourceFile != null) SyncSymbolCaches(sourceFile);
			RefreshFiles(destination.Location);
		}

		public FileSystemPath SavePreprocessResults(IT4File file, string text)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessAllowed();
			FileSystemPath destinationLocation = null;
			IProjectFile destination = null;
			Solution.InvokeUnderTransaction(cookie =>
			{
				RemoveLastGenOutput(file, cookie);
				string destinationName = GetPreprocessingTargetFileName(file);
				destination = GetOrCreateSameDestinationFile(cookie, file, destinationName);
				destinationLocation = destination.Location;
				destinationLocation.WriteAllText(text);
				UpdateLastGetOutput(file, destinationLocation, cookie);
			});
			SyncDocuments(destinationLocation);
			var sourceFile = destination.ToSourceFile();
			if (sourceFile != null) SyncSymbolCaches(sourceFile);
			RefreshFiles(destinationLocation);
			return destinationLocation;
		}

		protected virtual void SyncDocuments([NotNull] FileSystemPath destinationLocation)
		{
		}

		protected virtual void RefreshFiles([NotNull] FileSystemPath destinationLocation)
		{
		}

		private void SyncSymbolCaches([NotNull] IPsiSourceFile changedFile)
		{
			var changeManager = Solution.GetPsiServices().GetComponent<ChangeManager>();
			var invalidateCacheChange = new InvalidateCacheChange(
				Solution.GetComponent<SymbolCache>(),
				new[] {changedFile},
				true);

			using (WriteLockCookie.Create())
			{
				changeManager.OnProviderChanged(Solution, invalidateCacheChange, SimpleTaskExecutor.Instance);
			}
		}
	}
}
