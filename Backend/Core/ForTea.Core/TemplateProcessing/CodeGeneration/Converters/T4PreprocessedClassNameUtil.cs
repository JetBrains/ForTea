using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
  public static class T4PreprocessedClassNameUtil
  {
    [NotNull]
    public static string CreateGeneratedClassName([NotNull] this IPsiSourceFile sourceFile)
    {
      string fileName = sourceFile.Name.WithoutExtension();
      if (ValidityChecker.IsValidIdentifier(fileName)) return fileName;
      return T4CSharpIntermediateConverterBase.GeneratedClassNameString;
    }
  }
}