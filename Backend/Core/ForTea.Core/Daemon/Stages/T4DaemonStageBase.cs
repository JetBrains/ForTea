using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
  /// <summary>Base class for every T4 daemon stage.</summary>
  public abstract class T4DaemonStageBase : IModernDaemonStage
  {
    public bool IsApplicable(IPsiSourceFile sourceFile, DaemonProcessKind processKind)
    {
      return sourceFile.GetTheOnlyPsiFile(T4Language.Instance) is IT4File;
    }

    public IEnumerable<IDaemonStageProcess> CreateProcess(
      IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind)
    {
      var psiFile = (IT4File)process.SourceFile.GetTheOnlyPsiFile(T4Language.Instance).NotNull();
      return new[] { CreateProcess(process, psiFile, settings) };
    }

    [NotNull]
    protected abstract IDaemonStageProcess CreateProcess(
      [NotNull] IDaemonProcess process,
      [NotNull] IT4File file,
      [NotNull] IContextBoundSettingsStore settings);
  }
}