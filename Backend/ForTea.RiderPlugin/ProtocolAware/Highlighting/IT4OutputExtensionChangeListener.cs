using JetBrains.Annotations;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting
{
	public interface IT4OutputExtensionChangeListener
	{
		void ExtensionChanged([CanBeNull] string newExtension);
	}
}
