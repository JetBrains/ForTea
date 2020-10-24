using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Utils;
using JetBrains.ReSharper.Feature.Services.EditorConfig;
using JetBrains.Serialization;
using JetBrains.Util;
using JetBrains.Util.PersistentMap;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
	public sealed class T4LightWeightAssemblyResolutionDataMarshaller :
		IUnsafeMarshaller<T4LightWeightAssemblyResolutionData>
	{
		private static IUnsafeMarshaller<Dictionary<string, FileSystemPath>> PathToStringMarshaller { get; } =
			T4UnsafeMarshallers.GetDictionaryMarshaller(
				UnsafeMarshallers.UnicodeStringMarshaller,
				UnsafeMarshallers.FileSystemPathMarshaller
			);

		private T4LightWeightAssemblyResolutionDataMarshaller()
		{
		}

		public static IUnsafeMarshaller<T4LightWeightAssemblyResolutionData> Instance { get; } =
			new T4LightWeightAssemblyResolutionDataMarshaller();

		public void Marshal(UnsafeWriter writer, [NotNull] T4LightWeightAssemblyResolutionData value) =>
			PathToStringMarshaller.Marshal(writer, value.ResolvedAssemblies.ToDictionary());

		[NotNull]
		public T4LightWeightAssemblyResolutionData Unmarshal(UnsafeReader reader) =>
			new T4LightWeightAssemblyResolutionData(PathToStringMarshaller.Unmarshal(reader));
	}
}
