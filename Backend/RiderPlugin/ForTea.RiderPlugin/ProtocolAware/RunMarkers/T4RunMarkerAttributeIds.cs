using JetBrains.TextControl.DocumentMarkup;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.RunMarkers
{
  [RegisterHighlighter(
    RUN_T4_FILE_MARKER_ID,
    GutterMarkType = typeof(T4FileRunMarkerGutterMark),
    EffectType = EffectType.GUTTER_MARK,
    Layer = HighlighterLayer.SYNTAX + 1
  )]
  public static class T4RunMarkerAttributeIds
  {
    public const string RUN_T4_FILE_MARKER_ID = "T4 Run File Gutter Mark";
  }
}