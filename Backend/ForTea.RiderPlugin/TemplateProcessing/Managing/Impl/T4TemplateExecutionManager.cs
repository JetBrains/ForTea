using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debugger.Common.MetadataAndPdb;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
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
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public sealed class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private IPsiModules PsiModules { get; }

		[NotNull]
		private ISolutionProcessStartInfoPatcher Patcher { get; }

		[NotNull]
		private IT4TargetFileManager Manager { get; }

		[NotNull]
		private T4TreeNavigator Navigator { get; }

		[NotNull]
		private RoslynMetadataReferenceCache Cache { get; }

		[NotNull]
		private IT4BuildMessageConverter Converter { get; }

		public T4TemplateExecutionManager(
			Lifetime lifetime,
			[NotNull] IShellLocks locks,
			[NotNull] IPsiModules psiModules,
			[NotNull] ISolutionProcessStartInfoPatcher patcher,
			[NotNull] IT4TargetFileManager manager,
			[NotNull] IT4BuildMessageConverter converter,
			[NotNull] T4TreeNavigator navigator,
			[NotNull] ISolution solution
		)
		{
			Locks = locks;
			PsiModules = psiModules;
			Patcher = patcher;
			Manager = manager;
			Converter = converter;
			Navigator = navigator;
			Solution = solution;
			Cache = new RoslynMetadataReferenceCache(lifetime);
		}

		public T4BuildResult Compile(Lifetime lifetime, IT4File file, IProgressIndicator progress = null)
		{
			if (file.ContainsErrorElement()) return Converter.SyntaxError(Navigator.GetErrorElements(file).First());
			List<Diagnostic> messages = null;
			return lifetime.UsingNested(nested =>
			{
				try
				{
					var info = GenerateCode(file, nested);
					if (progress != null) progress.CurrentItemText = "Compiling code";
					var executablePath = Manager.GetTemporaryExecutableLocation(file);
					var compilation = CreateCompilation(info, executablePath);
					var diagnostics = compilation.GetDiagnostics(nested);
					messages = diagnostics.AsList();
					var errors = diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
					if (!errors.IsEmpty()) return null;

					executablePath.Parent.CreateDirectory();
					var pdbPath = executablePath.Parent.Combine(executablePath.Name.WithOtherExtension("pdb"));
					compilation.Emit(executablePath.FullPath, pdbPath.FullPath, cancellationToken: nested);
					CopyAssemblies(info, executablePath);
					return null;
				}
				catch (T4OutputGenerationException e)
				{
					return Converter.ToT4BuildResult(e);
				}
			}) ?? Converter.ToT4BuildResult(messages, file);
		}

		private T4TemplateExecutionManagerInfo GenerateCode([NotNull] IT4File file, Lifetime lifetime)
		{
			using (ReadLockCookie.Create())
			{
				var generator = new T4CSharpExecutableCodeGenerator(file, Solution);
				string code = generator.Generate().RawText;
				var references = ExtractReferences(file, lifetime);
				return new T4TemplateExecutionManagerInfo(code, references, file);
			}
		}

		private IEnumerable<T4MetadataReferenceInfo> ExtractReferences([NotNull] IT4File file, Lifetime lifetime)
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
					.SelectNotNull(it => T4MetadataReferenceInfo.FromPath(lifetime, it, Cache))
					.AsList();
			}
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
				references: info.References.Select(it => it.Reference));
		}

		private static void CopyAssemblies(
			T4TemplateExecutionManagerInfo info,
			[NotNull] FileSystemPath executablePath
		)
		{
			var folder = executablePath.Parent;
			IEnumerable<FileSystemPath> query = info
				.References
				.SelectNotNull(it => it.Path)
				.Where(it => !it.IsEmpty);
			foreach (var path in query)
			{
				path.CopyFile(folder.Combine(path.Name), true);
			}
		}

		public IT4ExecutionResult Execute(Lifetime lifetime, IT4File file, IProgressIndicator progress = null)
		{
			var executablePath = Manager.GetTemporaryExecutableLocation(file);
			string targetFileName = Manager.GetTargetFileName(file);
			var destinationPath = executablePath.Directory.Combine(targetFileName);
			var process = LaunchProcess(lifetime, executablePath, destinationPath);
			lifetime.ThrowIfNotAlive();
			process.WaitForExitSpinning(100, progress);
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

		public bool CanCompile(IT4File file)
		{
			var sourceFile = file.GetSourceFile();
			var cSharpFile = sourceFile?.GetPsiFiles(CSharpLanguage.Instance).SingleOrDefault();
			var t4File = sourceFile?.GetPsiFiles(T4Language.Instance).SingleOrDefault();
			if (cSharpFile?.ContainsErrorElement() != false) return false;
			if (t4File?.ContainsErrorElement() != false) return false;
			return true;
		}
	}
}
