using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Daemon.Attributes
{
	public interface IT4AttributeIdsProvider
	{
		[NotNull]
		string DirectiveId { get; }

		[NotNull]
		string AttributeId { get; }

		[NotNull]
		string AttributeValueId { get; }
	}
}
