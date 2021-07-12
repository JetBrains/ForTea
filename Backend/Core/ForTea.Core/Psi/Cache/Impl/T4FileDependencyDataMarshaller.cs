using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Serialization;
using JetBrains.Util;
using JetBrains.Util.PersistentMap;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	/// Note that a marshaller for <see cref="T4ReversedFileDependencyData"/> does not exist.
	/// That class is not intended to be persisted
	public sealed class T4FileDependencyDataMarshaller : IUnsafeMarshaller<T4FileDependencyData>
	{
		private IUnsafeMarshaller<IList<FileSystemPath>> PathListMarshaller { get; } =
			UnsafeMarshallers.GetCollectionMarshaller<FileSystemPath, IList<FileSystemPath>>(
				UnsafeMarshallers.FileSystemPathComparableMarshaller,
				size => new List<FileSystemPath>(size)
			);

		private T4FileDependencyDataMarshaller()
		{
		}

		[NotNull]
		public static T4FileDependencyDataMarshaller Instance { get; } = new T4FileDependencyDataMarshaller();

		public void Marshal([NotNull] UnsafeWriter writer, [NotNull] T4FileDependencyData value) =>
			PathListMarshaller.Marshal(writer, value.Includes);

		[NotNull]
		public T4FileDependencyData Unmarshal([NotNull] UnsafeReader reader) =>
			new T4FileDependencyData(PathListMarshaller.Unmarshal(reader));
	}
}
