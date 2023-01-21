using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.GeneratorKind
{
  public interface IT4GeneratorKind
  {
    string ProvideTemplateNamespace([NotNull] IT4File file);
  }

  /// <summary>
  /// Generates code that will be placed directly in the user solution
  /// (or used to provide editing support for a template that would do that)
  /// </summary>
  public sealed class T4PreprocessedGeneratorKind : IT4GeneratorKind
  {
    public string ProvideTemplateNamespace(IT4File file) => file
      .LogicalPsiSourceFile
      .ToProjectFile()
      ?.CalculateExpectedNamespace(T4Language.Instance);
  }

  /// <summary>
  /// Generates code that will be executed
  /// (or used to provide editing support for a template that would do that)
  /// </summary>
  public sealed class T4ExecutableGeneratorKind : IT4GeneratorKind
  {
    public string ProvideTemplateNamespace(IT4File file) => "JetBrains.Rider.TextTemplating";
  }
}