using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Services.CodeCompletion;
using JetBrains.Annotations;
using JetBrains.Application.CommandProcessing;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.ActionSystem.Text;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.StructuralRemove;
using JetBrains.ReSharper.Feature.Services.TypingAssist;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CachingLexers;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.TextControl;

namespace GammaJul.ForTea.Core.Services.TypingAssist
{
  [SolutionComponent]
  public class T4TypingAssist : TypingAssistLanguageBase<T4Language>, ITypingHandler
  {
    private static IReadOnlySet<TokenNodeType> AttributeValueTokens { get; } = new JetHashSet<TokenNodeType>
    {
      T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE,
      T4TokenNodeTypes.DOLLAR,
      T4TokenNodeTypes.LEFT_PARENTHESIS,
      T4TokenNodeTypes.RIGHT_PARENTHESIS,
      T4TokenNodeTypes.PERCENT
    };

    [NotNull] private readonly ICodeCompletionSessionManager _codeCompletionSessionManager;

    protected override bool IsSupported(ITextControl textControl)
    {
      IPsiSourceFile psiSourceFile = textControl.Document.GetPsiSourceFile(Solution);
      return psiSourceFile?.LanguageType.Is<T4ProjectFileType>() == true && psiSourceFile.IsValid();
    }

    public bool QuickCheckAvailability(ITextControl textControl, IPsiSourceFile projectFile)
      => projectFile.LanguageType.Is<T4ProjectFileType>();

    /// <summary>When = is typed, insert "".</summary>
    private bool OnEqualTyped(ITypingContext context)
    {
      ITextControl textControl = context.TextControl;

      // get the token type before =
      CachingLexer cachingLexer = GetCachingLexer(textControl);
      int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
      if (cachingLexer == null || offset <= 0 || !cachingLexer.FindTokenAt(offset - 1))
        return false;

      // do nothing if we're not after an attribute name
      TokenNodeType tokenType = cachingLexer.TokenType;
      if (tokenType != T4TokenNodeTypes.TOKEN)
        return false;

      // insert =
      textControl.Selection.Delete();
      textControl.FillVirtualSpaceUntilCaret();
      textControl.Document.InsertText(offset, "=");
      textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);

      // insert ""
      context.QueueCommand(() =>
      {
        using (CommandProcessor.UsingCommand("Inserting \"\""))
        {
          textControl.Document.InsertText(offset + 1, "\"\"");
          textControl.Caret.MoveTo(offset + 2, CaretVisualPlacement.DontScrollIfVisible);
        }

        // ignore if a subsequent " is typed by the user
        SkippingTypingAssist.SetCharsToSkip(textControl.Document, "\"");

        // popup auto completion
        _codeCompletionSessionManager.ExecuteAutoCompletion<T4AutopopupSettingsKey>(textControl, Solution,
          key => key.InDirectives);
      });

      return true;
    }

    private bool OnQuoteTyped([NotNull] ITypingContext context)
    {
      var textControl = context.TextControl;
      var cachingLexer = GetCachingLexer(textControl);
      int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
      if (cachingLexer == null || offset <= 0 || !cachingLexer.FindTokenAt(offset)) return false;
      var tokenType = cachingLexer.TokenType;

      // C# is handled by T4CSharpTypingAssist. Text tokens are handled by the platform
      if (tokenType is ICSharpTokenNodeType || tokenType == T4TokenNodeTypes.RAW_TEXT) return false;

      // The second quote has already been inserted. Over-type it
      if (tokenType == T4TokenNodeTypes.QUOTE)
      {
        textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
        return true;
      }

      // insert the first quote
      textControl.Selection.Delete();
      textControl.FillVirtualSpaceUntilCaret();
      textControl.Document.InsertText(offset, "\"");

      // insert the second quote
      using (CommandProcessor.UsingCommand("Smart quote in T4"))
      {
        textControl.Document.InsertText(offset + 1, "\"");
        textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
      }

      _codeCompletionSessionManager.ExecuteAutoCompletion<T4AutopopupSettingsKey>(textControl, Solution,
        key => key.InDirectives);
      return true;
    }

    /// <summary>When a # is typed, complete code block</summary>
    private bool OnOctothorpeTyped(ITypingContext context)
    {
      if (IsInsertingBlockStart(context))
      {
        InsertBlock(context);
        return true;
      }

      if (IsInsertingBlockEnd(context))
      {
        InsertBlockEnd(context);
        return true;
      }

      return false;
    }

    private void InsertBlock([NotNull] ITypingContext context)
    {
      var textControl = context.TextControl;
      int offset = textControl.GetOffset();
      InsertOctothorpe(textControl);
      context.QueueCommand(() =>
      {
        using (CommandProcessor.UsingCommand("Inserting T4 Code Block"))
        {
          textControl.Document.InsertText(offset + 1, "#>");
          textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
        }

        SkippingTypingAssist.SetCharsToSkip(textControl.Document, ">");
      });
    }

    private bool IsInsertingBlockStart([NotNull] ITypingContext context)
    {
      var textControl = context.TextControl;
      var lexer = GetCachingLexer(textControl);
      if (lexer == null) return false;
      int offset = textControl.GetOffset();
      if (!lexer.FindTokenAt(offset - 1)) return false;
      string tokenText = lexer.GetTokenText();
      return tokenText.EndsWith("<", StringComparison.Ordinal)
             && !tokenText.EndsWith("\\<", StringComparison.Ordinal);
    }

    private void InsertBlockEnd([NotNull] ITypingContext context)
    {
      var textControl = context.TextControl;
      int offset = textControl.GetOffset();
      InsertOctothorpe(textControl);
      context.QueueCommand(() =>
      {
        using (CommandProcessor.UsingCommand("Inserting >"))
        {
          textControl.Document.InsertText(offset + 1, ">");
          textControl.Caret.MoveTo(offset + 2, CaretVisualPlacement.DontScrollIfVisible);
        }

        SkippingTypingAssist.SetCharsToSkip(textControl.Document, ">");
      });
    }

    private bool IsInsertingBlockEnd([NotNull] ITypingContext context)
    {
      var textControl = context.TextControl;
      var lexer = GetCachingLexer(textControl);
      if (lexer == null) return false;
      int offset = textControl.GetOffset();
      var previousToken = FindPreviousToken(GetCachingLexer(textControl), offset);
      switch (previousToken)
      {
        case null:
        case T4TokenNodeType:
          return false;
        default:
          return true;
      }
    }

    private static void InsertOctothorpe(ITextControl textControl)
    {
      int offset = textControl.GetOffset();
      textControl.Selection.Delete();
      textControl.FillVirtualSpaceUntilCaret();
      textControl.Document.InsertText(offset, "#");
      textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
    }

    [CanBeNull]
    private TokenNodeType FindPreviousToken([CanBeNull] CachingLexer lexer, int initialOffset)
    {
      if (lexer == null) return null;
      for (int offset = initialOffset; offset >= 0; offset -= 1)
      {
        if (!lexer.FindTokenAt(offset)) continue;
        var tokenType = lexer.TokenType;
        if (tokenType?.IsWhitespace != false) continue;
        return tokenType;
      }

      return null;
    }

    private bool OnEnterPressed(IActionContext context)
    {
      var textControl = context.TextControl;
      int charPos;
      var lexer = GetCachingLexer(textControl);
      if (lexer == null) return false;
      if (!CheckAndDeleteSelectionIfNeeded(textControl,
            selection =>
            {
              charPos = TextControlToLexer(textControl, selection.StartOffset);
              if (charPos <= 0) return false;
              if (!lexer.FindTokenAt(charPos - 1)) return false;
              if (!IsBlockStart(lexer)) return false;
              if (!lexer.FindTokenAt(charPos)) return false;
              if (!IsBlockEnd(lexer)) return false;
              return true;
            })
         ) return false;
      int offset = textControl.GetOffset();
      var psiSourceFile = textControl.Document.GetPsiSourceFile(Solution);
      // Only insert one newline, as another gets inserted automatically
      textControl.Document.InsertText(offset, GetNewLineText(psiSourceFile));
      textControl.Caret.MoveTo(offset, CaretVisualPlacement.DontScrollIfVisible);
      return false;
    }

    private bool IsBlockEnd(CachingLexer lexer) => lexer.TokenType == T4TokenNodeTypes.BLOCK_END;

    private bool IsBlockStart([NotNull] CachingLexer lexer) =>
      lexer.TokenType == T4TokenNodeTypes.STATEMENT_BLOCK_START ||
      lexer.TokenType == T4TokenNodeTypes.EXPRESSION_BLOCK_START ||
      lexer.TokenType == T4TokenNodeTypes.FEATURE_BLOCK_START;

    private bool OnDollarTyped(ITypingContext context)
    {
      var textControl = context.TextControl;
      if (!IsInAttributeValue(textControl)) return false;

      int offset = textControl.GetOffset();
      textControl.Selection.Delete();
      textControl.Document.InsertText(offset, "$");
      textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
      context.QueueCommand(() =>
      {
        using (CommandProcessor.UsingCommand("Inserting T4 Macro Parenthesis"))
        {
          textControl.Document.InsertText(offset + 1, "()");
          textControl.Caret.MoveTo(offset + 2, CaretVisualPlacement.DontScrollIfVisible);
        }

        SkippingTypingAssist.SetCharsToSkip(textControl.Document, "(");
      });
      return true;
    }

    private bool IsInAttributeValue([NotNull] ITextControl textControl)
    {
      var lexer = GetCachingLexer(textControl);
      int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
      if (lexer == null || offset <= 1) return false;
      if (!lexer.FindTokenAt(offset - 1)) return false;
      var tokenType = lexer.TokenType;
      if (AttributeValueTokens.Contains(tokenType)) return true;
      if (tokenType != T4TokenNodeTypes.QUOTE) return false;
      if (!lexer.FindTokenAt(offset)) return false;
      return tokenType == T4TokenNodeTypes.QUOTE || tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE;
    }

    // When '%' is typed, insert another
    private bool OnPercentTyped(ITypingContext context)
    {
      var textControl = context.TextControl;
      if (!IsInAttributeValue(textControl)) return false;

      // get the token type after %
      var lexer = GetCachingLexer(textControl);
      int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
      if (lexer == null || offset <= 0 || !lexer.FindTokenAt(offset)) return false;

      // If there is already another percent after the %, swallow the typing
      var tokenType = lexer.TokenType;
      if (tokenType == T4TokenNodeTypes.PERCENT)
      {
        textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
        return true;
      }

      // insert the first %
      textControl.Selection.Delete();
      textControl.FillVirtualSpaceUntilCaret();
      textControl.Document.InsertText(offset, "%");

      // insert the second "
      context.QueueCommand(() =>
      {
        using (CommandProcessor.UsingCommand("Inserting %"))
        {
          textControl.Document.InsertText(offset + 1, "%");
          textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
        }
      });

      return true;
    }

    private bool OnClosingParenthesisTyped([NotNull] ITypingContext context)
    {
      var textControl = context.TextControl;
      if (!IsInAttributeValue(textControl)) return false;

      // Get the token type after caret
      var lexer = GetCachingLexer(textControl);
      int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
      if (lexer == null || offset <= 0 || !lexer.FindTokenAt(offset)) return false;
      var tokenType = lexer.TokenType;

      // If it's not a parenthesis, we don't care, so let the IDE handle it
      if (tokenType != T4TokenNodeTypes.RIGHT_PARENTHESIS) return false;

      // If it is, over-type it
      textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
      return true;
    }

    public T4TypingAssist(
      Lifetime lifetime,
      [NotNull] ISolution solution,
      [NotNull] ISettingsStore settingsStore,
      [NotNull] CachingLexerService cachingLexerService,
      [NotNull] ICommandProcessor commandProcessor,
      [NotNull] IPsiServices psiServices,
      [NotNull] IExternalIntellisenseHost externalIntellisenseHost,
      [NotNull] SkippingTypingAssist skippingTypingAssist,
      [NotNull] ITypingAssistManager typingAssistManager,
      [NotNull] ICodeCompletionSessionManager codeCompletionSessionManager,
      [NotNull] LastTypingAction lastTypingAction,
      [NotNull] StructuralRemoveManager structuralRemoveManager
    ) : base(
      solution,
      settingsStore,
      cachingLexerService,
      commandProcessor,
      psiServices,
      externalIntellisenseHost,
      skippingTypingAssist,
      lastTypingAction,
      structuralRemoveManager
    )
    {
      _codeCompletionSessionManager = codeCompletionSessionManager;

      typingAssistManager.AddTypingHandler(lifetime, '=', this, OnEqualTyped, IsTypingSmartParenthesisHandlerAvailable);
      typingAssistManager.AddTypingHandler(lifetime, '"', this, OnQuoteTyped, IsTypingSmartParenthesisHandlerAvailable);
      typingAssistManager.AddTypingHandler(lifetime, '#', this, OnOctothorpeTyped,
        IsTypingSmartParenthesisHandlerAvailable);
      typingAssistManager.AddTypingHandler(lifetime, '$', this, OnDollarTyped,
        IsTypingSmartParenthesisHandlerAvailable);
      typingAssistManager.AddTypingHandler(lifetime, '%', this, OnPercentTyped,
        IsTypingSmartParenthesisHandlerAvailable);
      typingAssistManager.AddActionHandler(lifetime, TextControlActions.ActionIds.Enter, this, OnEnterPressed,
        IsActionHandlerAvailable);
      typingAssistManager.AddTypingHandler(lifetime, ')', this, OnClosingParenthesisTyped,
        IsTypingSmartParenthesisHandlerAvailable);
    }
  }
}