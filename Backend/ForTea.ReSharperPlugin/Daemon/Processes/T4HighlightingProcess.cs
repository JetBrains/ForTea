using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Processes {

	/// <summary>Process that highlights block tags and missing token errors.</summary>
	internal sealed class T4HighlightingProcess : T4DaemonStageProcessBase {
		
		public override void ProcessBeforeInterior(ITreeNode element) {
			string attributeId = GetHighlightingAttributeId(element);
			if (attributeId != null) {
				DocumentRange range = element.GetHighlightingRange();
				AddHighlighting(range, new ReSharperSyntaxHighlighting(attributeId, string.Empty, range));
			}
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			if (!(element.GetTokenType() is T4TokenNodeType tokenType)) return null;
			if (tokenType.IsTag) return PredefinedHighlighterIds.HtmlServerSideScript;
			if (tokenType == T4TokenNodeTypes.QUOTE
			    || tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE
				||tokenType == T4TokenNodeTypes.EQUAL)
				return PredefinedHighlighterIds.HtmlAttributeName;

			return null;
		}
		
		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
			: base(file, daemonProcess) {
		}

	}

}
