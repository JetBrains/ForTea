using System.IO;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.Managing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	/// Note that this component is overridden in
	/// <see cref="JetBrains.ForTea.RiderPlugin.ProtocolAware.T4TargetFileManager">protocol</see> namespace 
	[SolutionComponent]
	public sealed class T4TargetFileManager : IT4TargetFileManager
	{
		[NotNull]
		private ISolution Solution { get; }

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
				.ToList()
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
			var destinationLocation = GetDestinationLocation(file, temporary.Name);
			destinationLocation.DeleteFile();
			File.Move(temporary.FullPath, destinationLocation.FullPath);
			Solution.InvokeUnderTransaction(cookie =>
			{
				var destination = UpdateProjectModel(file, destinationLocation, cookie);
				RemoveLastGenOutputIfDifferent(file, cookie, destinationLocation);
				UpdateLastGenOutput(file, destinationLocation, cookie);
				UpdateGeneratedFileStatus(file, destination, cookie);
			});
		}

		private void UpdateLastGenOutput(
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

		private void UpdateGeneratedFileStatus(
			[NotNull] IT4File source,
			[NotNull] IProjectFile destination,
			[NotNull] IProjectModelTransactionCookie cookie
		) => cookie.EditFileProperties(destination, properties =>
		{
			var projectFile = source.GetSourceFile()?.ToProjectFile();
			if (projectFile == null) return;
			if (!(properties is ProjectFileProperties projectFileProperties)) return;
			projectFileProperties.IsCustomToolOutput = true;
			projectFileProperties.IsDesignTimeBuildInput = true;
			projectFileProperties.DependsUponName = projectFile.Name;
		});

		private void RemoveLastGenOutputIfDifferent(
			[NotNull] IT4File file,
			[NotNull] IProjectModelTransactionCookie cookie,
			[NotNull] FileSystemPath destinationLocation
		)
		{
			var projectFile = file.GetSourceFile()?.ToProjectFile();
			if (projectFile == null) return;
			if (!(projectFile.Properties is ProjectFileProperties properties)) return;
			string output = properties.CustomToolOutput;
			var folder = projectFile.ParentFolder;
			if (folder == null) return;
			var suspects = folder
				.GetSubItems(output)
				.ToList()
				.OfType<IProjectFile>()
				.Where(it => TargetFileChecker.IsGeneratedFrom(it, projectFile))
				.Where(it => it.Location != destinationLocation);
			foreach (var suspect in suspects)
			{
				cookie.Remove(suspect);
			}
		}

		[NotNull]
		private IProjectFile UpdateProjectModel(
			[NotNull] IT4File file,
			[NotNull] FileSystemPath result,
			[NotNull] IProjectModelTransactionCookie cookie
		)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessAllowed();
			return GetOrCreateSameDestinationFile(cookie, file, result);
		}

		public FileSystemPath SavePreprocessResults(IT4File file, string text)
		{
			Locks.AssertReadAccessAllowed();
			Locks.AssertWriteAccessAllowed();
			FileSystemPath destinationLocation = null;
			IProjectFile destination;
			Solution.InvokeUnderTransaction(cookie =>
			{
				string destinationName = GetPreprocessingTargetFileName(file);
				destination = GetOrCreateSameDestinationFile(cookie, file, destinationName);
				destinationLocation = destination.Location;
				destinationLocation.WriteAllText(text);
				RemoveLastGenOutputIfDifferent(file, cookie, destinationLocation);
				UpdateLastGenOutput(file, destinationLocation, cookie);
			});
			return destinationLocation;
		}
	}
}
