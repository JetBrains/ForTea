using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Attributes
{
	[SolutionComponent]
	public sealed class T4ReSharperAttributeIdsProvider : IT4AttributeIdsProvider
	{
		public string DirectiveId => PredefinedHighlighterIds.HtmlElementName;
		public string AttributeId => PredefinedHighlighterIds.HtmlAttributeName;
		public string AttributeValueId => PredefinedHighlighterIds.HtmlAttributeValue;
	}
}
