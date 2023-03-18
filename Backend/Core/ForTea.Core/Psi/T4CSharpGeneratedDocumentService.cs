using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Web.Generation;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi
{
  /// <summary>This class will generate a C# code-behind from a T4 file.</summary>
  [GeneratedDocumentService(typeof(T4ProjectFileType))]
  [ZoneMarker(typeof(IWebPsiLanguageZone))]
  public sealed class T4CSharpGeneratedDocumentService : GeneratedDocumentServiceBase
  {
    private static IEnumerable<PsiLanguageType> PsiLanguageTypes => new PsiLanguageType[] { CSharpLanguage.Instance };

    /// <summary>Generates a C# file from a T4 file.</summary>
    /// <param name="modificationInfo">The modifications that occurred in the T4 file.</param>
    public override ISecondaryDocumentGenerationResult Generate(PrimaryFileModificationInfo modificationInfo)
    {
      if (!(modificationInfo.NewPsiFile is IT4File t4File)) return null;
      if (!T4DirectiveInfoManager.GetLanguageType(t4File).Is<CSharpLanguage>()) return null;
      var result = T4CodeGeneration.GenerateCodeBehind(t4File);
      var csharpLanguageService = CSharpLanguage.Instance.LanguageService();
      if (csharpLanguageService == null) return null;

      return new SecondaryDocumentGenerationResult(
        result.RawText,
        csharpLanguageService.LanguageType,
        new RangeTranslatorWithGeneratedRangeMap(result.GeneratedRangeMap),
        csharpLanguageService.GetPrimaryLexerFactory()
      );
    }

    /// <summary>Gets the secondary PSI language types for a T4 file.</summary>
    /// <returns>Always <see cref="CSharpLanguage"/>.</returns>
    public override IEnumerable<PsiLanguageType> GetSecondaryPsiLanguageTypes(IProject project) => PsiLanguageTypes;

    public override bool IsSecondaryPsiLanguageType(IProject project, PsiLanguageType language)
      => language.Is<CSharpLanguage>();

    /// <summary>Creates a secondary lexing service for code behind generated files.</summary>
    /// <param name="solution">The solution.</param>
    /// <param name="mixedLexer">The mixed lexer.</param>
    /// <param name="sourceFile">The source file.</param>
    /// <returns>An instance of <see cref="ISecondaryLexingProcess"/> used to lex the code behind file.</returns>
    public override ISecondaryLexingProcess CreateSecondaryLexingService(
      ISolution solution,
      MixedLexer mixedLexer,
      IPsiSourceFile sourceFile = null
    ) => new T4SecondaryLexingProcess(CSharpLanguage.Instance, mixedLexer);

    /// <summary>Gets a lexer factory capable of handling preprocessor directives.</summary>
    /// <param name="primaryLanguage">The primary language.</param>
    /// <returns>Always <c>null</c> since there is no preprocessor directives in T4 files.</returns>
    public override ILexerFactory LexerFactoryWithPreprocessor(PsiLanguageType primaryLanguage)
      => null;

    /// <summary>Reparses the original T4 file.</summary>
    /// <param name="treeTextRange">The tree text range to reparse.</param>
    /// <param name="newText">The new text to add at <paramref name="treeTextRange"/>.</param>
    /// <param name="rangeTranslator">The range translator.</param>
    /// <returns><c>true</c> if reparse succeeded, <c>false</c> otherwise.</returns>
    protected override bool ReparseOriginalFile(
      TreeTextRange treeTextRange,
      string newText,
      RangeTranslatorWithGeneratedRangeMap rangeTranslator
    ) => rangeTranslator.OriginalFile is IT4File t4File
         && t4File.ReParse(treeTextRange, newText) != null;

    /// <summary>
    /// The process of generated document commit (in the case of primary document incremental reparse)
    /// can be overridden in this method.
    /// Returns null if full regeneration is required.
    /// This method is not allowed to do destructive changes due to interruptibility!
    /// </summary>
    public override ICollection<ICommitBuildResult> ExecuteSecondaryDocumentCommitWork(
      PrimaryFileModificationInfo primaryFileModificationInfo,
      CachedPsiFile cachedPsiFile,
      TreeTextRange oldTreeRange,
      string newText
    )
    {
      var rangeTranslator = (RangeTranslatorWithGeneratedRangeMap)cachedPsiFile.PsiFile.SecondaryRangeTranslator;
      if (rangeTranslator == null)
        return null;

      TreeTextRange range = rangeTranslator.OriginalToGenerated(oldTreeRange, JetPredicate<IUserDataHolder>.True);
      DocumentRange documentRange = cachedPsiFile.PsiFile.DocumentRangeTranslator.Translate(range);
      if (!documentRange.IsValid())
        return null;

      var documentChange = new DocumentChange(documentRange.Document, documentRange.TextRange.StartOffset,
        documentRange.TextRange.Length, newText,
        documentRange.Document.LastModificationStamp, TextModificationSide.NotSpecified);

      return new ICommitBuildResult[]
      {
        new CommitBuildResult(cachedPsiFile.WorkIncrementalParse(documentChange), null, documentChange, null,
          TextRange.InvalidRange, String.Empty),
        new FixRangeTranslatorsOnSharedRangeCommitBuildResult(rangeTranslator, null,
          new TreeTextRange<Original>(oldTreeRange), new TreeTextRange<Generated>(range), newText)
      };
    }
  }
}