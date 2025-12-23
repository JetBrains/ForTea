using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using static GammaJul.ForTea.Core.Parsing.T4TokenNodeFlag;

namespace GammaJul.ForTea.Core.Parsing
{
  [Language(typeof(T4Language))]
  public partial class T4TokenNodeTypes : INodeTypesInitializer
  {
    [NotNull] public static readonly T4TokenNodeType DIRECTIVE_START
      = new T4TokenNodeType("DIRECTIVE_START", DIRECTIVE_START_NODE_TYPE_INDEX, "<#@", Tag);

    [NotNull] public static readonly T4TokenNodeType STATEMENT_BLOCK_START
      = new T4TokenNodeType("STATEMENT_BLOCK_START", STATEMENT_BLOCK_START_NODE_TYPE_INDEX, "<#", Tag);

    [NotNull] public static readonly T4TokenNodeType EXPRESSION_BLOCK_START
      = new T4TokenNodeType("EXPRESSION_BLOCK_START", EXPRESSION_BLOCK_START_NODE_TYPE_INDEX, "<#=", Tag);

    [NotNull] public static readonly T4TokenNodeType FEATURE_BLOCK_START
      = new T4TokenNodeType("FEATURE_BLOCK_START", FEATURE_BLOCK_START_NODE_TYPE_INDEX, "<#+", Tag);

    [NotNull] public static readonly T4TokenNodeType BLOCK_END
      = new T4TokenNodeType("BLOCK_END", BLOCK_END_NODE_TYPE_INDEX, "#>", Tag);

    [NotNull] public static readonly T4TokenNodeType RAW_TEXT
      = new T4TokenNodeType("RAW_TEXT", RAW_TEXT_NODE_TYPE_INDEX, null, None);

    [NotNull] public static readonly T4TokenNodeType RAW_CODE
      = new T4TokenNodeType("RAW_CODE", RAW_CODE_NODE_TYPE_INDEX, null, None);

    [NotNull] public static readonly T4TokenNodeType RAW_ATTRIBUTE_VALUE
      = new T4TokenNodeType("RAW_ATTRIBUTE_VALUE", RAW_ATTRIBUTE_VALUE_NODE_TYPE_INDEX, null, StringLiteral);

    [NotNull] public static readonly T4TokenNodeType WHITE_SPACE
      = new T4TokenNodeType("WHITE_SPACE", WHITE_SPACE_NODE_TYPE_INDEX, " ", Whitespace);

    [NotNull] public static readonly T4TokenNodeType NEW_LINE
      = new T4TokenNodeType("NEW_LINE", NEW_LINE_NODE_TYPE_INDEX, "\n", Whitespace);

    [NotNull] public static readonly T4TokenNodeType QUOTE
      = new T4TokenNodeType("QUOTE", QUOTE_NODE_TYPE_INDEX, "\"", None);

    [NotNull] public static readonly T4TokenNodeType EQUAL
      = new T4TokenNodeType("EQUAL", EQUAL_NODE_TYPE_INDEX, "=", None);

    [NotNull] public static readonly T4TokenNodeType TOKEN
      = new T4TokenNodeType("TOKEN", TOKEN_NODE_TYPE_INDEX, null, Identifier);

    [NotNull] public static readonly T4TokenNodeType OUTPUT
      = new T4TokenNodeType("OUTPUT", OUTPUT_NODE_TYPE_INDEX, "output", Identifier);

    [NotNull] public static readonly T4TokenNodeType IMPORT
      = new T4TokenNodeType("IMPORT", IMPORT_NODE_TYPE_INDEX, "import", Identifier);

    [NotNull] public static readonly T4TokenNodeType INCLUDE
      = new T4TokenNodeType("INCLUDE", INCLUDE_NODE_TYPE_INDEX, "include", Identifier);

    [NotNull] public static readonly T4TokenNodeType TEMPLATE
      = new T4TokenNodeType("TEMPLATE", TEMPLATE_NODE_TYPE_INDEX, "template", Identifier);

    [NotNull] public static readonly T4TokenNodeType ASSEMBLY
      = new T4TokenNodeType("ASSEMBLY", ASSEMBLY_NODE_TYPE_INDEX, "assembly", Identifier);

    [NotNull] public static readonly T4TokenNodeType PARAMETER
      = new T4TokenNodeType("PARAMETER", PARAMETER_NODE_TYPE_INDEX, "parameter", Identifier);

    [NotNull] public static readonly T4TokenNodeType CLEANUP_BEHAVIOR = new T4TokenNodeType("CLEANUP_BEHAVIOR",
      CLEANUP_BEHAVIOR_NODE_TYPE_INDEX, "CleanupBehavior", Identifier);

    [NotNull] public static readonly T4TokenNodeType UNKNOWN_DIRECTIVE_NAME
      = new T4TokenNodeType("UNKNOWN", UNKNOWN_DIRECTIVE_NAME_NODE_TYPE_INDEX, null, Identifier);

    [NotNull] public static readonly T4TokenNodeType BAD_TOKEN
      = new T4TokenNodeType("BAD_TOKEN", BAD_TOKEN_NODE_TYPE_INDEX, null, None);

    [NotNull] public static readonly T4TokenNodeType LEFT_PARENTHESIS
      = new T4TokenNodeType("LEFT_PARENTHESIS", LEFT_PARENTHESIS_NODE_TYPE_INDEX, "(", None);

    [NotNull] public static readonly T4TokenNodeType DOLLAR
      = new T4TokenNodeType("DOLLAR", DOLLAR_NODE_TYPE_INDEX, "$", None);

    [NotNull] public static readonly T4TokenNodeType RIGHT_PARENTHESIS
      = new T4TokenNodeType("RIGHT_PARENTHESIS", RIGHT_PARENTHESIS_NODE_TYPE_INDEX, ")", None);

    [NotNull] public static readonly T4TokenNodeType PERCENT
      = new T4TokenNodeType("PERCENT", PERCENT_NODE_TYPE_INDEX, "%", None);

    [NotNull] public static readonly NodeTypeSet CodeBlockStarts
      = new NodeTypeSet(STATEMENT_BLOCK_START, EXPRESSION_BLOCK_START, FEATURE_BLOCK_START);
  }
}