using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public sealed class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull] private const string DefaultExecutableExtension = "exe";
		[NotNull] private const string DefaultExecutableExtensionWithDot = "." + DefaultExecutableExtension;
		[NotNull] private const string Title = "Executing T4 Template";

		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		private Lifetime SolutionLifetime { get; }

		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private IPsiModules PsiModules { get; }

		[NotNull]
		private ISolutionProcessStartInfoPatcher Patcher { get; }
		
		[NotNull]
		private IT4TargetFileManager Manager { get; }

		public T4TemplateExecutionManager(
			Lifetime solutionLifetime,
			[NotNull] IShellLocks locks,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] IPsiModules psiModules,
			[NotNull] ISolutionProcessStartInfoPatcher patcher,
			[NotNull] IT4TargetFileManager manager)
		{
			SolutionLifetime = solutionLifetime;
			Locks = locks;
			DirectiveInfoManager = directiveInfoManager;
			PsiModules = psiModules;
			Patcher = patcher;
			Manager = manager;
		}

		public IT4ExecutionResult Execute(IT4File file, IProgressIndicator progress = null)
		{
			var definition = SolutionLifetime.CreateNested();
			LaunchProgress(progress);
			var info = GenerateCode(file, progress);
			return CompileAndRun(definition, info);
		}

		private static void LaunchProgress([CanBeNull] IProgressIndicator indicator)
		{
			if (indicator == null) return;
			indicator.Start(1);
			indicator.Advance();
			indicator.TaskName = Title;
			indicator.CurrentItemText = "Preparing";
		}

		private T4TemplateExecutionManagerInfo GenerateCode(
			[NotNull] IT4File file,
			[CanBeNull] IProgressIndicator progress
		)
		{
			if (progress != null) progress.CurrentItemText = "Generating code";
			using (ReadLockCookie.Create())
			{
				var timeStamp = file.GetSourceFile().NotNull().LastWriteTimeUtc;
				var generator = new T4CSharpExecutableCodeGenerator(file, DirectiveInfoManager);
				string code = generator.Generate().RawText;
				var references = ExtractReferences(file);
				return new T4TemplateExecutionManagerInfo(code, references, file, progress);
			}
		}

		private IEnumerable<MetadataReference> ExtractReferences([NotNull] IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var psiModule = sourceFile.PsiModule;
			var resolveContext = psiModule.GetResolveContextEx(projectFile);
			using (CompilationContextCookie.GetOrCreate(resolveContext))
			{
				return PsiModules
					.GetModuleReferences(psiModule)
					.Select(it => it.Module)
					.OfType<IAssemblyPsiModule>()
					.Select(it => it.Assembly)
					.SelectNotNull(it => it.Location)
					.Select(it => MetadataReference.CreateFromFile(it.FullPath));
			}
		}

		private IT4ExecutionResult CompileAndRun(LifetimeDefinition definition, T4TemplateExecutionManagerInfo info)
		{
			if (info.ProgressIndicator != null) info.ProgressIndicator.CurrentItemText = "Compiling code";
			var executablePath = CreateTemporaryExecutable(definition.Lifetime);
			var compilation = CreateCompilation(info, executablePath);
			var errors = compilation
				.GetDiagnostics(definition.Lifetime)
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.ToList();
			if (!errors.IsEmpty())
			{
				MessageBox.ShowError(errors.Select(error => error.ToString()).Join("\n"), "Could not compile template");
				return new T4ExecutionFailure();
			}

			var pdbPath = executablePath.Parent.Combine(executablePath.Name.WithOtherExtension("pdb"));
			compilation.Emit(executablePath.FullPath, pdbPath.FullPath, cancellationToken: definition.Lifetime);
			CopyAssemblies(info, executablePath);
			return Run(info, definition.Lifetime, executablePath);
		}

		private CSharpCompilation CreateCompilation(T4TemplateExecutionManagerInfo info, FileSystemPath executablePath)
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				info.Code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

			return CSharpCompilation.Create(
				executablePath.Name,
				new[] {syntaxTree},
				options: options,
				references: info.References);
		}

		private void CopyAssemblies(
			T4TemplateExecutionManagerInfo info,
			[NotNull] FileSystemPath executablePath
		)
		{
			var folder = executablePath.Parent;
			IEnumerable<FileSystemPath> query = info
				.References
				.SelectNotNull(it => it.Display)
				.SelectNotNull(it => FileSystemPath.TryParse(it));
			foreach (var path in query)
			{
				File.Copy(path.FullPath, folder.Combine(path.Name).FullPath);
			}
		}

		private IT4ExecutionResult Run(
			T4TemplateExecutionManagerInfo info,
			Lifetime lifetime,
			FileSystemPath executablePath
		)
		{
			string targetFileName = Manager.GetTargetFileName(info.File);
			var destinationPath = executablePath.Directory.Combine(targetFileName);
			var process = LaunchProcess(lifetime, executablePath, destinationPath);
			lifetime.ThrowIfNotAlive();
			process.WaitForExitSpinning(100, info.ProgressIndicator);
			lifetime.ThrowIfNotAlive();
			return new T4ExecutionResultInFile(destinationPath);
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

		[NotNull]
		private static FileSystemPath CreateTemporaryExecutable(Lifetime lifetime)
		{
			var directory = FileSystemDefinition.CreateTemporaryDirectory();
			return FileSystemDefinition.CreateTemporaryFile(lifetime, directory, DefaultExecutableExtensionWithDot);
		}
	}
}
