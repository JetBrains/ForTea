using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Format;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util.Text;

namespace GammaJul.ForTea.Core.Psi.Service.Impl
{
  [Language(typeof(T4Language))]
  internal class T4CodeFormatter : CodeFormatterBase<T4CodeFormattingSettingsKey>, IT4CodeFormatter
  {
    public T4CodeFormatter(PsiLanguageType languageType, CodeFormatterRequirements requirements) : base(
      languageType, requirements)
    {
    }

    protected override CodeFormattingContext CreateFormatterContext(
      AdditionalFormatterParameters parameters,
      ICustomFormatterInfoProvider provider,
      int tabWidth,
      SingleLangChangeAccu changeAccu,
      FormatTask[] formatTasks)
      => new CodeFormattingContext(this, FormatterLoggerProvider.FormatterLogger, parameters, tabWidth, changeAccu, formatTasks);

    public override MinimalSeparatorType GetMinimalSeparatorByNodeTypes(
      TokenNodeType leftToken,
      TokenNodeType rightToken
    ) => MinimalSeparatorType.NotRequired;

		public override ITreeNode CreateSpace(string indent, NodeType replacedOrLeftSiblingType) => new T4Token(
      T4TokenNodeTypes.WHITE_SPACE,
      new StringBuffer(indent),
      TreeOffset.Zero,
      new TreeOffset(indent.Length)
    );

    public override ITreeNode CreateNewLine(LineEnding lineEnding, NodeType lineBreakType = null) =>
      new T4Token(
        T4TokenNodeTypes.NEW_LINE,
        new StringBuffer(lineEnding.GetPresentation()),
        TreeOffset.Zero,
        new TreeOffset(lineEnding.GetPresentation().Length)
      );

    public override ITreeRange Format(
      ITreeNode firstElement,
      ITreeNode lastElement,
      CodeFormatProfile profile,
      AdditionalFormatterParameters parameters = null
    ) => null;

    public override void FormatInsertedNodes(ITreeNode nodeFirst, ITreeNode nodeLast, bool formatSurround)
    {
    }

    public override ITreeRange FormatInsertedRange(ITreeNode nodeFirst, ITreeNode nodeLast, ITreeRange origin) =>
      null;

    public override void FormatReplacedNode(ITreeNode oldNode, ITreeNode newNode)
    {
    }

    public override void FormatReplacedRange(ITreeNode first, ITreeNode last, ITreeRange oldNodes)
    {
    }

    public override void FormatDeletedNodes(ITreeNode parent, ITreeNode prevNode, ITreeNode nextNode)
    {
    }
  }
}