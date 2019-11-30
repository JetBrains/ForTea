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
		private IUnsafeMarshaller<IList<string>> StringListMarshaller { get; } =
			UnsafeMarshallers.GetCollectionMarshaller<string, IList<string>>(
				UnsafeMarshallers.UnicodeStringMarshaller,
				size => new List<string>(size)
			);

		private T4FileDependencyDataMarshaller()
		{
		}

		[NotNull]
		public static T4FileDependencyDataMarshaller Instance { get; } = new T4FileDependencyDataMarshaller();

		public void Marshal([NotNull] UnsafeWriter writer, [NotNull] T4FileDependencyData value) =>
			StringListMarshaller.Marshal(writer, value.Paths.Select(includee => includee.FullPath).AsList());

		[NotNull]
		public T4FileDependencyData Unmarshal([NotNull] UnsafeReader reader) =>
			new T4FileDependencyData(StringListMarshaller
				.Unmarshal(reader)
				.Select(path => FileSystemPath.Parse(path))
				.AsList());
	}
}
