using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
  [ProjectFileType(typeof(T4ProjectFileType))]
  internal sealed class T4CSharpCustomFormattingInfoProvider : DummyCSharpCustomFormattingInfoProvider
  {
    private IEnumerable<IT4BlockSpaceTypeProvider> Providers { get; }

    public T4CSharpCustomFormattingInfoProvider(IEnumerable<IT4BlockSpaceTypeProvider> providers) =>
      Providers = providers;

    public override bool NeedsOldEngine => true;

    public override FmtSettingsClassic<CSharpFormatSettingsKey> AdjustFormattingSettings(
      [NotNull] FmtSettingsClassic<CSharpFormatSettingsKey> settings,
      [NotNull] ISettingsOptimization settingsOptimization
    )
    {
      var cSharpFormatSettings = settings.Settings.Clone();
      cSharpFormatSettings.OLD_ENGINE = true;
      return settings.ChangeMainSettings(cSharpFormatSettings, true);
    }

    public override SpaceType GetBlockSpaceType(
      [NotNull] CSharpFmtStageContext ctx,
      [NotNull] CSharpCodeFormattingContext context
    ) => GetSpaces(ctx);

    public override SpaceType GetInvocationSpaces(
      CSharpFmtStageContext context,
      FmtSettingsClassic<CSharpFormatSettingsKey> formatSettings
    ) => GetSpaces(context);

    private SpaceType GetSpaces(CSharpFmtStageContext ctx) => Providers
      .Select(provider => provider.Provide(ctx))
      .Where(provided => provided.HasValue)
      .Select(provided => provided.Value)
      // default(SpaceType) == SpaceType.Default
      .FirstOrDefault();
  }
}