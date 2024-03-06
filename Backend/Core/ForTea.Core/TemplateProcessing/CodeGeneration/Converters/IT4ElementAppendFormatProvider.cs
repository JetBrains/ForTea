using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
  public interface IT4ElementAppendFormatProvider
  {
    string ToStringConversionPrefix { get; }
    string ToStringConversionSuffix { get; }
    string ExpressionWritingPrefix { get; }
    string ExpressionWritingSuffix { get; }
    string CodeCommentStart { get; }
    string CodeCommentEnd { get; }
    string ExpressionCommentStart { get; }
    string ExpressionCommentEnd { get; }
    string Indent { get; }
    bool ShouldBreakExpressionWithLineDirective { get; }
    bool ShouldUseLineDirectives { get; }

    void AppendCompilationOffset(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4TreeNode node
    );

    void AppendLineDirective(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4TreeNode node
    );

    // TODO: wtf
    void AppendMappedIfNeeded(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4Code code
    );
  }
}