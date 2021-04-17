using System;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;
using GammaJul.ForTea.Core.Parsing.Token;

%%

%namespace GammaJul.ForTea.Core.Parsing.Lexing
%class T4LexerGenerated
%implements IIncrementalLexer
%type TokenNodeType

%init{
  myCurrentTokenType = null;
%init}

%function advance
%unicode

%eofval{
  myCurrentTokenType = null;
  return myCurrentTokenType;
%eofval}

%include T4Rules.lex
