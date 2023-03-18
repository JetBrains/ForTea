using GammaJul.ForTea.Core.Psi;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.Tests.Highlighting
{
  public abstract class T4HighlightingTestBase : HighlightingTestBase
  {
    protected sealed override bool HighlightingPredicate(
      IHighlighting highlighting,
      IPsiSourceFile sourceFile,
      IContextBoundSettingsStore settingsStore
    )
    {
      var instance = HighlightingSettingsManager.Instance;
      var severity = instance.GetSeverity(highlighting, sourceFile, Solution, settingsStore);
      return severity == Target;
    }

    protected abstract Severity Target { get; }
    protected sealed override PsiLanguageType CompilerIdsLanguage => T4Language.Instance;
  }
}