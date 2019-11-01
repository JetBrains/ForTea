using System.Collections.Generic;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Rider.Model;
using JetBrains.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public sealed class T4TemplateCompiler : IT4TemplateCompiler
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private IT4TargetFileManager TargetManager { get; }

		[NotNull]
		private IT4ReferenceExtractionManager ReferenceExtractionManager { get; }

		[NotNull]
		private IT4BuildMessageConverter Converter { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4TemplateCompiler(
			[NotNull] IShellLocks locks,
			[NotNull] IT4TargetFileManager targetManager,
			[NotNull] IT4BuildMessageConverter converter,
			[NotNull] ISolution solution,
			[NotNull] IT4ReferenceExtractionManager referenceExtractionManager,
			[NotNull] ILogger logger
		)
		{
			Locks = locks;
			TargetManager = targetManager;
			Converter = converter;
			Solution = solution;
			ReferenceExtractionManager = referenceExtractionManager;
			Logger = logger;
		}

		[NotNull]
		public T4BuildResult Compile(Lifetime lifetime, IPsiSourceFile sourceFile)
		{
			Logger.Verbose("Compiling a file");
			Locks.AssertReadAccessAllowed();
			// Since we need no context when compiling a file, we need to build the tree manually
			var file = sourceFile.BuildT4Tree();
			var error = file.ThisAndDescendants<IErrorElement>().Collect();
			if (!error.IsEmpty()) return Converter.SyntaxErrors(error);

			return lifetime.UsingNested(nested =>
			{
				try
				{
					// Prepare the code
					var references = ReferenceExtractionManager.ExtractPortableReferencesTransitive(lifetime, file);
					string code = GenerateCode(file);

					// Prepare the paths
					var executablePath = TargetManager.GetTemporaryExecutableLocation(file);
					var compilation = CreateCompilation(code, references, executablePath);
					executablePath.Parent.CreateDirectory();
					var pdbPath = executablePath.Parent.Combine(executablePath.Name.WithOtherExtension("pdb"));

					// Delegate to Roslyn
					var emitOptions = new EmitOptions(
						debugInformationFormat: DebugInformationFormat.PortablePdb,
						pdbFilePath: pdbPath.FullPath
					);
					EmitResult emitResult;
					using (var executableStream = executablePath.OpenFileForWriting())
					{
						using (var pdbStream = pdbPath.OpenFileForWriting())
						{
							emitResult = compilation.Emit(
								peStream: executableStream,
								pdbStream: pdbStream,
								options: emitOptions,
								cancellationToken: nested
							);
						}
					}

					return Converter.ToT4BuildResult(emitResult.Diagnostics.AsList(), file);
				}
				catch (T4OutputGenerationException e)
				{
					return Converter.ToT4BuildResult(e);
				}
			});
		}

		[NotNull]
		private string GenerateCode([NotNull] IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			var generator = new T4CSharpExecutableCodeGenerator(file, Solution);
			string code = generator.Generate().RawText;
			return code;
		}

		private static CSharpCompilation CreateCompilation(
			[NotNull] string code,
			[NotNull] IEnumerable<MetadataReference> references,
			[NotNull] FileSystemPath executablePath
		)
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

			return CSharpCompilation.Create(
				executablePath.Name,
				new[] {syntaxTree},
				options: options,
				references: references);
		}
	}
}
