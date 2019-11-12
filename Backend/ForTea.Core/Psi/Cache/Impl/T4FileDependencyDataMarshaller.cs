using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Serialization;
using JetBrains.Util;
using JetBrains.Util.PersistentMap;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	public sealed class T4FileDependencyDataMarshaller : IUnsafeMarshaller<T4FileDependencyData>
	{
		private T4FileDependencyDataMarshaller()
		{
		}

		[NotNull]
		public static T4FileDependencyDataMarshaller Instance { get; } = new T4FileDependencyDataMarshaller();

		public void Marshal([NotNull] UnsafeWriter writer, [NotNull] T4FileDependencyData value) =>
			WriteList(writer, value.Paths.Select(includee => includee.FullPath).ToList());

		[NotNull]
		public T4FileDependencyData Unmarshal([NotNull] UnsafeReader reader) =>
			new T4FileDependencyData(ReadList(reader).Select(path => FileSystemPath.Parse(path)).AsList());

		private static void WriteList([NotNull] UnsafeWriter writer, [NotNull, ItemNotNull] IList<string> list)
		{
			writer.Write(list.Count);
			foreach (string value in list)
			{
				writer.Write(value);
			}
		}

		[NotNull, ItemNotNull]
		private static IList<string> ReadList([NotNull] UnsafeReader reader)
		{
			int count = reader.ReadInt32();
			var list = new List<string>();
			for (int i = 0; i < count; i++)
			{
				list.Add(reader.ReadString());
			}

			return list;
		}
	}
}
