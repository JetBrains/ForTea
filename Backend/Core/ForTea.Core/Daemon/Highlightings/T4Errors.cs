using GammaJul.ForTea.Core.Resources;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
  [RegisterStaticHighlightingsGroup(typeof(Strings), nameof(Strings.T4Errors_Text), true)]
  public class T4Errors
  {
  }
}