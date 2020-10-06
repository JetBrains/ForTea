WHITESPACE=[\ \n\r\t\f]+
LETTER=[A-Za-z_]
TOKEN={LETTER}+

RAW_CODE=([^#]|(#+[^#>]))+
RAW_TEXT=([^<\r\n]|(<+[^<#\r\n])|(\\<#))+
RAW_ATTRIBUTE_VALUE=[^$%()\"]+

%state IN_DIRECTIVE
%state IN_BLOCK
%state IN_ATTRIBUTE_VALUE
%state IN_DIRECTIVE_NAME
