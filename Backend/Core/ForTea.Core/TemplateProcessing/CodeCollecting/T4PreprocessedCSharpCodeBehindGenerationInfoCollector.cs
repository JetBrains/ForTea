using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
  public sealed class T4PreprocessedCSharpCodeBehindGenerationInfoCollector :
    T4CSharpCodeBehindGenerationInfoCollector
  {
    [NotNull] private IPsiSourceFile Root { get; }

    [NotNull] private IT4FileDependencyGraph Graph { get; }

    private bool IsRootFile { get; }

    public T4PreprocessedCSharpCodeBehindGenerationInfoCollector(
      [NotNull] ISolution solution,
      [NotNull] IPsiSourceFile root,
      [NotNull] IT4FileDependencyGraph graph,
      bool isRootFile
    ) : base(solution)
    {
      Root = root;
      Graph = graph;
      IsRootFile = isRootFile;
    }

    protected override void AppendFeature(IT4Code code, IT4AppendableElementDescription description)
    {
      if (code.IsVisibleInDocumentUnsafe())
      {
        Result.AppendFeature(description);
        return;
      }

      // We are generating partial class for preprocessed template.
      // It would be very convenient to generate partial classes for every T4 file
      // with every class containing all and only features defined in that file.
      // However, that's not always possible because includes can be included into several files.
      // Therefore we have to take into account whether the current root file
      // is selected as the root for that include file.
      var containingFile = (IT4File)code.GetContainingFile().NotNull();
      if (Graph.FindBestRoot(containingFile.LogicalPsiSourceFile) == Root)
      {
        // If they share the same root, the include will generate
        // a partial class with all the features in it,
        // so we should not generate anything here to avoid resolution errors.
        return;
      }

      // Otherwise it will generate a part for some other class,
      // which means that the only way we will get class features
      // is if we generate them in the root file of this preprocessed template tree
      if (!IsRootFile) return;
      Result.AppendFeature(description);
    }
  }
}