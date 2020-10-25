using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl
{
	public static class T4OutputExtensionChangeListenerEx
	{
		[NotNull]
		private static readonly Key<T4OutputExtensionChangeListener> T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY =
			new Key<T4OutputExtensionChangeListener>("T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY");

		public static void CreateOutputExtensionChangeListener(
			[NotNull] this IDocument thіs,
			Lifetime lifetime,
			[NotNull] T4OutputExtensionChangeListener listener
		) => lifetime.Bracket(
			() => thіs.PutData(T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY, listener),
			() => thіs.PutData(T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY, null)
		);

		[CanBeNull]
		public static T4OutputExtensionChangeListener GetOutputExtensionChangeListener([NotNull] this IDocument thіs) =>
			thіs.GetData(T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY);
	}
}
