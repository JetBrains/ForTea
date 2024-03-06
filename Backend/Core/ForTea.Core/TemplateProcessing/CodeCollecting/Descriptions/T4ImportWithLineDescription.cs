using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
  public sealed class T4ImportWithLineDescription : T4ImportDescription
  {
    private T4ImportWithLineDescription([NotNull] IT4TreeNode source) : base(source)
    {
    }

    [CanBeNull]
    public new static T4ImportWithLineDescription FromDirective([NotNull] IT4Directive directive)
    {
      (var source, string _) =
        directive.GetAttributeValueIgnoreOnlyWhitespace(T4DirectiveInfoManager.Import.NamespaceAttribute.Name);
      if (source == null) return null;
      return new T4ImportWithLineDescription(source);
    }

    public override void AppendContent(
      T4CSharpCodeGenerationResult destination,
      IT4ElementAppendFormatProvider provider
    )
    {
      provider.AppendLineDirective(destination, Source);
      provider.AppendCompilationOffset(destination, Source);
      destination.Append("using ");
      destination.AppendMapped(Source);
      destination.AppendLine(";");
      destination.Append(provider.Indent);
      destination.AppendLine("#line default");
      destination.Append(provider.Indent);
      destination.AppendLine("#line hidden");
    }
  }
}