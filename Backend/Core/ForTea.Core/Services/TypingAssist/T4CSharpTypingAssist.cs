using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Components;
using JetBrains.DocumentModel.Transactions;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CSharp.TypingAssist;
using JetBrains.ReSharper.Feature.Services.TypingAssist;
using JetBrains.ReSharper.Feature.Services.Web.TypingAssist;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.TextControl;

namespace GammaJul.ForTea.Core.Services.TypingAssist
{
  /// <summary>Typing assistant for C# embedded in T4 files.</summary>
  [SolutionComponent]
  [ZoneMarker(typeof(IWebPsiLanguageZone))]
  public sealed class T4CSharpTypingAssist : CSharpTypingAssistBase
  {
    protected override bool IsSupported(ITextControl textControl)
    {
      if (!WebTypingAssistUtil.IsProjectFileSupported<T4ProjectFileType, CSharpLanguage>(textControl, Solution))
        return false;
      if (WebTypingAssistUtil.IsSupported<ICSharpTokenNodeType>(GetCachingLexer(textControl), textControl))
        return true;
      return IsInEmptyCodeBlock(textControl);
    }

    private bool IsInEmptyCodeBlock([NotNull] ITextControl textControl)
    {
      var lexer = GetCachingLexer(textControl);
      if (lexer == null) return false;
      int caretOffset = textControl.Caret.Offset();
      if (!WebTypingAssistUtil.FindTokenAt(lexer, caretOffset)) return false;
      if (!(lexer.TokenType is T4TokenNodeType tokenType)) return false;
      if (tokenType != T4TokenNodeTypes.BLOCK_END) return false;
      if (caretOffset != lexer.TokenStart) return false;
      lexer.Advance(-1);
      var prevToken = lexer.TokenType;
      if (prevToken != null && T4TokenNodeTypes.CodeBlockStarts[prevToken]) return true;
      return false;
    }

    public override bool QuickCheckAvailability(ITextControl textControl, IPsiSourceFile projectFile) =>
      projectFile.LanguageType.Is<T4ProjectFileType>();

    public T4CSharpTypingAssist(
      Lifetime lifetime,
      [NotNull] TypingAssistDependencies dependencies,
      [NotNull] DocumentTransactionManager documentTransactionManager,
      [NotNull] IOptional<ICodeCompletionSessionManager> codeCompletionSessionManager
    ) : base(lifetime, dependencies, documentTransactionManager, codeCompletionSessionManager)
    {
    }
  }
}