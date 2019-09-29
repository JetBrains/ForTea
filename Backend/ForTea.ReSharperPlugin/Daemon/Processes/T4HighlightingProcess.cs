using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Attributes;
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
			if (tokenType.IsTag) return T4ReSharperCustomHighlightingIds.BLOCK_TAG;
			if (tokenType == T4TokenNodeTypes.QUOTE
			    || tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE
				||tokenType == T4TokenNodeTypes.EQUAL
			    || tokenType == T4TokenNodeTypes.DOLLAR
			    || tokenType == T4TokenNodeTypes.PERCENT
			    || tokenType == T4TokenNodeTypes.LEFT_PARENTHESIS
			    || tokenType == T4TokenNodeTypes.RIGHT_PARENTHESIS)
				return T4ReSharperCustomHighlightingIds.ATTRIBUTE_VALUE;
			if (T4Lexer.DirectiveTypes[tokenType]) return T4ReSharperCustomHighlightingIds.DIRECTIVE;
			if (tokenType == T4TokenNodeTypes.TOKEN) return T4ReSharperCustomHighlightingIds.DIRECTIVE_ATTRIBUTE;

			return null;
		}
		
		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
			: base(file, daemonProcess) {
		}

	}

}
