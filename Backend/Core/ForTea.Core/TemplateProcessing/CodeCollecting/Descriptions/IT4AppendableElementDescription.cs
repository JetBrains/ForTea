using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
  public interface IT4AppendableElementDescription
  {
    void AppendContent(
      [NotNull] T4CSharpCodeGenerationResult destination,
      [NotNull] IT4ElementAppendFormatProvider provider);
  }
}