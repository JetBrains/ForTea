using JetBrains.Annotations;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting
{
	public interface IT4OutputExtensionChangeListener
	{
		void ExtensionChanged([NotNull] string newExtension);
	}
}
