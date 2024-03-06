using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
  public class T4ExpressionDescription : IT4AppendableElementDescription
  {
    [NotNull] private IT4Code Source { get; }

    public T4ExpressionDescription([NotNull] IT4Code source) => Source = source;

    public void AppendContent(
      T4CSharpCodeGenerationResult destination,
      IT4ElementAppendFormatProvider provider
    )
    {
      if (!provider.ShouldBreakExpressionWithLineDirective) AppendAllContent(destination, provider);
      else AppendAllContentBreakingExpression(destination, provider);
    }

    private void AppendAllContentBreakingExpression(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      destination.AppendLine(provider.Indent);
      AppendContentPrefix(destination, provider);
      destination.AppendLine();
      AppendOpeningLineDirective(destination, provider);
      AppendMainContent(destination, provider);
      destination.AppendLine();
      AppendClosingLineDirective(destination, provider);
      AppendContentSuffix(destination, provider);
    }

    private void AppendAllContent(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      destination.AppendLine(provider.Indent);
      AppendOpeningLineDirective(destination, provider);
      AppendContentPrefix(destination, provider);
      AppendMainContent(destination, provider);
      AppendContentSuffix(destination, provider);
      destination.AppendLine(provider.Indent);
      AppendClosingLineDirective(destination, provider);
    }

    private static void AppendClosingLineDirective(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      destination.Append(provider.Indent);
      destination.AppendLine("#line default");
      destination.Append(provider.Indent);
      destination.AppendLine("#line hidden");
    }

    protected virtual void AppendContentSuffix(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      destination.Append(provider.ToStringConversionSuffix);
      destination.AppendLine(provider.ExpressionWritingSuffix);
    }

    private void AppendMainContent(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      destination.Append(provider.ExpressionCommentStart);
      provider.AppendCompilationOffset(destination, Source);
      provider.AppendMappedIfNeeded(destination, Source);
      destination.Append(provider.ExpressionCommentEnd);
    }

    private void AppendOpeningLineDirective(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      provider.AppendLineDirective(destination, Source);
    }

    protected virtual void AppendContentPrefix(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider
    )
    {
      destination.Append(provider.Indent);
      destination.Append(provider.ExpressionWritingPrefix);
      destination.Append(provider.ToStringConversionPrefix);
    }
  }
}