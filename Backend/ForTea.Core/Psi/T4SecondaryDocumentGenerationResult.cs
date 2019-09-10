using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Invalidation;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace GammaJul.ForTea.Core.Psi
{
	/// <summary>Specialization of <see cref="SecondaryDocumentGenerationResult"/> that add dependencies between a file and its includes.</summary>
	public sealed class T4SecondaryDocumentGenerationResult : SecondaryDocumentGenerationResult
	{
		[NotNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private IEnumerable<IT4IncludeDirective> IncludedFiles { get; }

		[NotNull]
		private T4FileDependencyManager T4FileDependencyManager { get; }

		public override void CommitChanges()
		{
			Logger.GetLogger<T4SecondaryDocumentGenerationResult>().Verbose("CommitChanges");
			var location = SourceFile.GetLocation();
			if (location.IsEmpty) return;
			var projectFile = SourceFile.ToProjectFile();
			IEnumerable<FileSystemPath> includePaths;
			if (projectFile == null)
			{
				includePaths = IncludedFiles
					.Select(include => include.Path.ResolvePath())
					.Where(path => !path.IsEmpty);
			}
			else
			{
				using (T4MacroResolveContextCookie.Create(projectFile))
				{
					includePaths = IncludedFiles
						.Select(include => include.Path.ResolvePath())
						.Where(path => !path.IsEmpty)
						.AsList();
				}
			}

			T4FileDependencyManager.UpdateIncludes(location, new HashSet<FileSystemPath>(includePaths));
			T4FileDependencyManager.TryGetCurrentInvalidator()?.AddCommittedFilePath(location);
		}

		public T4SecondaryDocumentGenerationResult(
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] string text,
			[NotNull] PsiLanguageType language,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator,
			[NotNull] ILexerFactory lexerFactory,
			[NotNull] T4FileDependencyManager t4FileDependencyManager,
			[NotNull] IEnumerable<IT4IncludeDirective> includedFiles
		) : base(text, language, secondaryRangeTranslator, lexerFactory)
		{
			SourceFile = sourceFile;
			T4FileDependencyManager = t4FileDependencyManager;
			IncludedFiles = includedFiles;
		}
	}
}
