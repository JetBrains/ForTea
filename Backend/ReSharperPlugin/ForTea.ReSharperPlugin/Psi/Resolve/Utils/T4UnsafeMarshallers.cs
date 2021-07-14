using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Collections;
using JetBrains.Util.PersistentMap;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Utils
{
	public static class T4UnsafeMarshallers
	{
		[NotNull]
		private static IUnsafeMarshaller<KeyValuePair<TKey, TValue>> GetKeyValuePairMarshaller<TKey, TValue>(
			IUnsafeMarshaller<TKey> keyMarshaller,
			IUnsafeMarshaller<TValue> valueMarshaller
		) => new UniversalMarshaller<KeyValuePair<TKey, TValue>>(
			reader => new KeyValuePair<TKey, TValue>(
				keyMarshaller.Unmarshal(reader),
				valueMarshaller.Unmarshal(reader)
			),
			(writer, pair) =>
			{
				var (key, value) = pair;
				keyMarshaller.Marshal(writer, key);
				valueMarshaller.Marshal(writer, value);
			}
		);

		[NotNull]
		public static IUnsafeMarshaller<Dictionary<TKey, TValue>> GetDictionaryMarshaller<TKey, TValue>(
			IUnsafeMarshaller<TKey> keyMarshaller,
			IUnsafeMarshaller<TValue> valueMarshaller
		) => UnsafeMarshallers.GetCollectionMarshaller(
			GetKeyValuePairMarshaller(keyMarshaller, valueMarshaller),
			size => new Dictionary<TKey, TValue>(size)
		);
	}
}
