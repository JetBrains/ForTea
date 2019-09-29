using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Attributes
{
	[SolutionComponent]
	public sealed class T4ReSharperAttributeIdsProvider : IT4AttributeIdsProvider
	{
		public string DirectiveId => T4ReSharperCustomHighlightingIds.DIRECTIVE;
		public string AttributeId => T4ReSharperCustomHighlightingIds.DIRECTIVE_ATTRIBUTE;
		public string AttributeValueId => T4ReSharperCustomHighlightingIds.ATTRIBUTE_VALUE;
	}
}
