using JetBrains.ForTea.RiderPlugin.ProtocolAware.RunMarkers;
using JetBrains.TextControl.DocumentMarkup;

[assembly:RegisterHighlighter(
	T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID,
	GutterMarkType = typeof(T4FileRunMarkerGutterMark),
	EffectType = EffectType.GUTTER_MARK,
	Layer = HighlighterLayer.SYNTAX + 1
)]

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.RunMarkers
{
	public static class T4RunMarkerAttributeIds
	{
		public const string RUN_T4_FILE_MARKER_ID = "T4 Run File Gutter Mark";
	}
}
