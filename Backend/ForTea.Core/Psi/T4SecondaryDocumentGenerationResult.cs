using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Invalidation;
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
		private HashSet<FileSystemPath> IncludedFiles { get; }

		[NotNull]
		private T4FileDependencyManager T4FileDependencyManager { get; }

		public override void CommitChanges()
		{
			Logger.GetLogger<T4SecondaryDocumentGenerationResult>().Verbose("CommitChanges");
			var location = SourceFile.GetLocation();
			if (location.IsEmpty) return;
			T4FileDependencyManager.UpdateIncludes(location, IncludedFiles);
			T4FileDependencyManager.TryGetCurrentInvalidator()?.AddCommittedFilePath(location);
		}

		public T4SecondaryDocumentGenerationResult(
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] string text,
			[NotNull] PsiLanguageType language,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator,
			[NotNull] ILexerFactory lexerFactory,
			[NotNull] T4FileDependencyManager t4FileDependencyManager,
			[NotNull] IEnumerable<FileSystemPath> includedFiles
		) : base(text, language, secondaryRangeTranslator, lexerFactory)
		{
			SourceFile = sourceFile;
			T4FileDependencyManager = t4FileDependencyManager;
			IncludedFiles = new HashSet<FileSystemPath>(includedFiles);
		}
	}
}
