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

namespace GammaJul.ForTea.Core.Psi
{
	/// <summary>Specialization of <see cref="SecondaryDocumentGenerationResult"/> that add dependencies between a file and its includes.</summary>
	public sealed class T4SecondaryDocumentGenerationResult : SecondaryDocumentGenerationResult
	{
		[NotNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private IEnumerable<IT4IncludeDirective> IncludeDirectives { get; }

		[NotNull, ItemNotNull]
		private IEnumerable<FileSystemPath> IncludedFiles => IncludeDirectives
			.Select(include => include.Path.ResolvePath())
			.Where(path => !path.IsEmpty);

		[NotNull]
		private T4FileDependencyManager T4FileDependencyManager { get; }

		public override void CommitChanges()
		{
			var location = SourceFile.GetLocation();
			if (location.IsEmpty) return;
			var projectFile = SourceFile.ToProjectFile();
			IEnumerable<FileSystemPath> includePaths;
			if (projectFile == null)
			{
				includePaths = IncludedFiles;
			}
			else
			{
				using (T4MacroResolveContextCookie.Create(projectFile))
				{
					includePaths = IncludedFiles.AsList();
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
			[NotNull] IEnumerable<IT4IncludeDirective> includeDirectives
		) : base(text, language, secondaryRangeTranslator, lexerFactory)
		{
			SourceFile = sourceFile;
			T4FileDependencyManager = t4FileDependencyManager;
			IncludeDirectives = includeDirectives;
		}
	}
}
