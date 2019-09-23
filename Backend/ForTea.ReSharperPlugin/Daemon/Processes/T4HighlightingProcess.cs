using GammaJul.ForTea.Core.Daemon.Attributes;
using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Processes {

	/// <summary>Process that highlights block tags and missing token errors.</summary>
	internal sealed class T4HighlightingProcess : T4DaemonStageProcessBase {
		
		public override void ProcessBeforeInterior(ITreeNode element) {
			string attributeId = GetHighlightingAttributeId(element);
			if (attributeId != null) {
				DocumentRange range = element.GetHighlightingRange();
				AddHighlighting(range, new PredefinedHighlighting(attributeId, range));
			}
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			if (!(element.GetTokenType() is T4TokenNodeType tokenType))
				return null;
			
			if (tokenType.IsTag)
				return T4ReSharperHighlightingAttributeIds.BLOCK_MARKER;

			if (tokenType == T4TokenNodeTypes.EQUAL)
				return T4ReSharperHighlightingAttributeIds.OPERATOR;

			if (tokenType == T4TokenNodeTypes.QUOTE || tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE)
				return T4HighlightingAttributeIds.RAW_ATTRIBUTE_VALUE;

			return null;
		}
		
		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
			: base(file, daemonProcess) {
		}

	}

}
