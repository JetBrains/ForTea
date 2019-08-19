using System;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;

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

WHITESPACE=[\ \n\r\t\f]+
LETTER=[A-Za-z_]
TOKEN={LETTER}+

TEMPLATE=[Tt][Ee][Mm][Pp][Ll][Aa][Tt][Ee]
PARAMETER=[Pp][Aa][Rr][Aa][Mm][Ee][Tt][Ee][Rr]
OUTPUT=[Oo][Uu][Tt][Pp][Uu][Tt]
ASSEMBLY=[Aa][Ss][Ss][Ee][Mm][Bb][Ll][Yy]
IMPORT=[Ii][Mm][Pp][Oo][Rr][Tt]
INCLUDE=[Ii][Nn][Cc][Ll][Uu][Dd][Ee]
CLEANUP_BEHAVIOR=[Cc][Ll][Ee][Aa][Nn][Aa][Pp][Bb][Ee][Hh][Aa][Vv][Ii][Oo][Uu]?[Rr]
UNKNOWN_DIRECTIVE_NAME={TOKEN}

RAW_CODE=([^#]|(#+[^#>]))+
RAW_TEXT=([^<\r\n]|(<+[^<#\r\n])|(\\<#))+
RAW_ATTRIBUTE_VALUE=[^$%()\"]+

%state IN_DIRECTIVE
%state IN_BLOCK
%state IN_ATTRIBUTE_VALUE
%state IN_DIRECTIVE_NAME
%%
<YYINITIAL> "<#@"                            { yybegin(IN_DIRECTIVE_NAME); myCurrentTokenType = makeToken(T4TokenNodeTypes.DIRECTIVE_START); return myCurrentTokenType; }
<YYINITIAL> "<#="                            { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.EXPRESSION_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "<#+"                            { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.FEATURE_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "<#"                             { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.STATEMENT_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "#>"                             { yybegin(YYINITIAL); myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<YYINITIAL> (\r|\n|\r\n)                     { myCurrentTokenType = makeToken(T4TokenNodeTypes.NEW_LINE); return myCurrentTokenType; }
<YYINITIAL> {RAW_TEXT}                       { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_TEXT); return myCurrentTokenType; }
<YYINITIAL> [^]                              { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_TEXT); return myCurrentTokenType; }

<IN_DIRECTIVE_NAME> {WHITESPACE}             { myCurrentTokenType = makeToken(T4TokenNodeTypes.WHITE_SPACE); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {TEMPLATE}               { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.TEMPLATE); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {PARAMETER}              { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.PARAMETER); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {OUTPUT}                 { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.OUTPUT); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {ASSEMBLY}               { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.ASSEMBLY); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {IMPORT}                 { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.IMPORT); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {INCLUDE}                { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.INCLUDE); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {CLEANUP_BEHAVIOR}       { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.CLEANUP_BEHAVIOR); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> {UNKNOWN_DIRECTIVE_NAME} { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.UNKNOWN_DIRECTIVE_NAME); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> "<#@"                    { yybegin(IN_DIRECTIVE_NAME); myCurrentTokenType = makeToken(T4TokenNodeTypes.DIRECTIVE_START); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> "<#="                    { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.EXPRESSION_BLOCK_START); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> "<#+"                    { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.FEATURE_BLOCK_START); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> "<#"                     { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.STATEMENT_BLOCK_START); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> "#>"                     { yybegin(YYINITIAL); myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<IN_DIRECTIVE_NAME> [^]                      { myCurrentTokenType = makeToken(T4TokenNodeTypes.BAD_TOKEN); return myCurrentTokenType; } 

<IN_DIRECTIVE> {WHITESPACE}                  { myCurrentTokenType = makeToken(T4TokenNodeTypes.WHITE_SPACE); return myCurrentTokenType; }
<IN_DIRECTIVE> {TOKEN}                       { myCurrentTokenType = makeToken(T4TokenNodeTypes.TOKEN); return myCurrentTokenType; }
<IN_DIRECTIVE> "="                           { myCurrentTokenType = makeToken(T4TokenNodeTypes.EQUAL); return myCurrentTokenType; }
<IN_DIRECTIVE> "\""                          { yybegin(IN_ATTRIBUTE_VALUE); myCurrentTokenType = makeToken(T4TokenNodeTypes.QUOTE); return myCurrentTokenType; }
<IN_DIRECTIVE> "<#@"                         { yybegin(IN_DIRECTIVE_NAME); myCurrentTokenType = makeToken(T4TokenNodeTypes.DIRECTIVE_START); return myCurrentTokenType; }
<IN_DIRECTIVE> "<#="                         { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.EXPRESSION_BLOCK_START); return myCurrentTokenType; }
<IN_DIRECTIVE> "<#+"                         { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.FEATURE_BLOCK_START); return myCurrentTokenType; }
<IN_DIRECTIVE> "<#"                          { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.STATEMENT_BLOCK_START); return myCurrentTokenType; }
<IN_DIRECTIVE> "#>"                          { yybegin(YYINITIAL); myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<IN_DIRECTIVE> [^]                           { myCurrentTokenType = makeToken(T4TokenNodeTypes.BAD_TOKEN); return myCurrentTokenType; }

<IN_ATTRIBUTE_VALUE> "\""                    { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.QUOTE); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> {RAW_ATTRIBUTE_VALUE}   { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> "$"                     { myCurrentTokenType = makeToken(T4TokenNodeTypes.DOLLAR); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> "("                     { myCurrentTokenType = makeToken(T4TokenNodeTypes.LEFT_PARENTHESIS); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> ")"                     { myCurrentTokenType = makeToken(T4TokenNodeTypes.RIGHT_PARENTHESIS); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> "%"                     { myCurrentTokenType = makeToken(T4TokenNodeTypes.PERCENT); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> [^]                     { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE); return myCurrentTokenType; }

<IN_BLOCK> "<#@"                             { yybegin(IN_DIRECTIVE_NAME); myCurrentTokenType = makeToken(T4TokenNodeTypes.DIRECTIVE_START); return myCurrentTokenType; }
<IN_BLOCK> "<#="                             { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.EXPRESSION_BLOCK_START); return myCurrentTokenType; }
<IN_BLOCK> "<#+"                             { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.FEATURE_BLOCK_START); return myCurrentTokenType; }
<IN_BLOCK> "<#"                              { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.STATEMENT_BLOCK_START); return myCurrentTokenType; }
<IN_BLOCK> "#>"                              { yybegin(YYINITIAL); myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<IN_BLOCK> {RAW_CODE}                        { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_CODE); return myCurrentTokenType; }
<IN_BLOCK> [^]                               { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_CODE); return myCurrentTokenType; }
