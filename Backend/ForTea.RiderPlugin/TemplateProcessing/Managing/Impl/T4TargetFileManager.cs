using System.IO;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
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
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		protected ISolution Solution { get; }

		[NotNull]
		private IShellLocks Locks => Solution.Locks;

		public T4TargetFileManager([NotNull] T4DirectiveInfoManager manager, [NotNull] ISolution solution)
		{
			Manager = manager;
			Solution = solution;
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

		public string GetExpectedTargetFileName(IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			string name = sourceFile.Name;
			string targetExtension = file.GetTargetExtension(Manager);
			return name.WithOtherExtension(targetExtension);
		}

		private FileSystemPath GetTemporaryTargetFileFolder(IT4File file) =>
			GetTemporaryExecutableLocation(file).Parent;

		private FileSystemPath FindTemporaryTargetFile(IT4File file)
		{
			string name = file.GetSourceFile().NotNull().Name.WithOtherExtension(".*");
			var candidates = GetTemporaryTargetFileFolder(file).GetChildFiles(name);
			Assertion.Assert(candidates.Count == 1, "candidates.Count == 1");
			return candidates.First();
		}

		private string GetPreprocessingTargetFileName(IT4File file)
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

		public FileSystemPath CopyExecutionResults(IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessForbidden();
			var temporary = FindTemporaryTargetFile(file);
			var destinationLocation = GetDestinationLocation(file, temporary.Name);
			// There seems to be no method with 'move-and-overwrite' semantics
			File.Copy(temporary.FullPath, destinationLocation.FullPath, true);
			File.Delete(temporary.FullPath);
			return destinationLocation;
		}

		public void UpdateProjectModel(IT4File file, FileSystemPath result)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessAllowed();
			IProjectFile destination = null;
			Solution.InvokeUnderTransaction(
				cookie => destination = GetOrCreateSameDestinationFile(cookie, file, result));
			// TODO: Do I really need that?
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
			string destinationName = GetPreprocessingTargetFileName(file);
			Solution.InvokeUnderTransaction(cookie =>
			{
				destination = GetOrCreateSameDestinationFile(cookie, file, destinationName);
				destinationLocation = destination.Location;
				destinationLocation.WriteAllText(text);
			});
			SyncDocuments(destinationLocation);
			var sourceFile = destination.ToSourceFile();
			if (sourceFile != null) SyncSymbolCaches(sourceFile);
			RefreshFiles(destinationLocation);
			return destinationLocation;
		}

		protected virtual void SyncDocuments(FileSystemPath destinationLocation)
		{
		}

		protected virtual void RefreshFiles(FileSystemPath destinationLocation)
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
