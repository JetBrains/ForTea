using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl
{
	public static class T4OutputExtensionChangeListenerEx
	{
		[NotNull]
		private static readonly Key<IT4OutputExtensionChangeListener> T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY =
			new Key<IT4OutputExtensionChangeListener>("T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY");

		public static void CreateListener(
			[NotNull] this IDocument thіs,
			Lifetime lifetime,
			[NotNull] IT4OutputExtensionChangeListener listener
		) => lifetime.Bracket(
			() => thіs.PutData(T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY, listener),
			() => thіs.PutData(T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY, null)
		);

		[NotNull]
		public static IT4OutputExtensionChangeListener GetListener([NotNull] this IDocument thіs) =>
			thіs.GetData(T4_OUTPUT_EXTENSION_CHANGE_LISTENER_KEY).NotNull();
	}
}
