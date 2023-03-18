using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.EditorConfig;
using JetBrains.ReSharper.Psi.Format;

namespace GammaJul.ForTea.Core.Psi.Service
{
  [SettingsKey(typeof(CodeFormattingSettingsKey), "Code formatting in T4")]
  [EditorConfigKey("T4")]
  public sealed class T4CodeFormattingSettingsKey : FormatSettingsKeyBase
  {
  }
}