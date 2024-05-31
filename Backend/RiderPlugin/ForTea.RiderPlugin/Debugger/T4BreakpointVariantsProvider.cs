using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.IDE.Debugger;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Debugger;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace JetBrains.ForTea.RiderPlugin.Debugger
{
  [Language(typeof(T4Language))]
  [ZoneMarker(typeof(IDebuggerZone))]
  public class T4BreakpointVariantsProvider : IBreakpointVariantsProvider
  {
    // TODO: delegate to C# provider
    [NotNull]
    public IEnumerable<string> GetSupportedFileExtensions() => T4ProjectFileType.AllExtensions.ToList();

    [CanBeNull]
    public IReadOnlyList<IBreakpoint> GetBreakpointVariants(
      [NotNull] IProjectFile file,
      int line,
      [NotNull] ISolution solution
    )
    {
      var variants = new List<IBreakpoint>();

      var sourceFile = solution.PsiModules().GetPsiSourceFilesFor(file).FirstOrDefault();
      if (sourceFile == null)
        return null;
      var document = sourceFile.Document;

      int lineCount = (int)document.GetLineCount();
      if (line > lineCount)
        return variants;

      variants.Add(new LineBreakpoint());
      return variants;
    }
  }
}