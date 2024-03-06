using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
  public sealed class T4TextDescription : IT4AppendableElementDescription
  {
    [NotNull] private string Text { get; }

    [CanBeNull] private IT4TreeNode FirstNode { get; }

    public T4TextDescription([NotNull] string text, [CanBeNull] IT4TreeNode firstNode = null)
    {
      Text = text;
      FirstNode = firstNode;
    }

    public void AppendContent(
      T4CSharpCodeGenerationResult destination,
      IT4ElementAppendFormatProvider provider
    )
    {
      if (FirstNode != null)
      {
        if (provider.ShouldUseLineDirectives) destination.AppendLine(provider.Indent);
        provider.AppendLineDirective(destination, FirstNode);
      }
      else
      {
        destination.Append(provider.Indent);
      }

      destination.Append(provider.ExpressionWritingPrefix);
      destination.Append("\"");
      destination.Append(Sanitize(Text));
      destination.Append("\"");
      destination.AppendLine(provider.ExpressionWritingSuffix);
      if (FirstNode != null)
      {
        destination.AppendLine();
        if (provider.ShouldUseLineDirectives)
        {
          destination.AppendLine(provider.Indent);
          destination.Append(provider.Indent);
          destination.AppendLine("#line default");
          destination.Append(provider.Indent);
          destination.AppendLine("#line hidden");
        }
      }
    }

    [NotNull]
    private static string Sanitize([NotNull] string raw) => raw.Replace("\\\\<#", "<#").Replace("\\\\#>", "#>");
  }
}