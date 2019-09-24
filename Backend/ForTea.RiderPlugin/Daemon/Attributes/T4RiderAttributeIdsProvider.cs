using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Attributes
{
	[SolutionComponent]
	public sealed class T4RiderAttributeIdsProvider : IT4AttributeIdsProvider
	{
		public string DirectiveId => T4RiderHighlightAttributeIds.DIRECTIVE;
		public string AttributeId => T4RiderHighlightAttributeIds.DIRECTIVE_ATTRIBUTE;
		public string AttributeValueId => T4RiderHighlightAttributeIds.RAW_ATTRIBUTE_VALUE;
	}
}
