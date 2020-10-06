package com.jetbrains.fortea.lexer;

import com.intellij.psi.tree.IElementType;
import com.intellij.lexer.FlexLexer;

import com.jetbrains.fortea.psi.T4TokenNodeTypes;

%%

%{
  private IElementType myCurrentTokenType;
  private IElementType makeToken(IElementType token) {
    myCurrentTokenType = token;
    return myCurrentTokenType;
  }

  private IElementType FindDirectiveByCurrentToken() {
      return T4TokenNodeTypes.TOKEN;
  }
%}

%class _T4Lexer
%implements FlexLexer
%type IElementType
%init{
  myCurrentTokenType = null;
%init}

%function advance
%unicode

%eofval{
    myCurrentTokenType = null;
    return null;
%eofval}

%include T4Rules.lex

%%
%include T4Transitions.lex
