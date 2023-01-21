using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi.Search;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
  /// <summary>
  /// An implementation of <see cref="IOccurrenceProvider" /> that creates occurrences for find results that are
  /// inside included files that aren't present in the current solution, and thus ignored by ReSharper.
  /// </summary>
  [OccurrenceProvider(Priority = 10000)]
  public sealed class T4OutsideSolutionOccurrenceProvider : IOccurrenceProvider
  {
    public IOccurrence MakeOccurrence(FindResult findResult)
    {
      if (!(findResult is FindResultText findResultText)) return null;
      if (findResultText.DocumentRange.Document.GetOutsideSolutionPath().IsEmpty) return null;
      var rangeMarker = findResultText
        .Solution
        .GetComponent<DocumentManager>()
        .CreateRangeMarker(findResultText.DocumentRange);
      return new T4OutsideSolutionOccurrence(rangeMarker);
    }
  }
}