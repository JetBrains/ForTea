using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Utils;
using JetBrains.Serialization;
using JetBrains.Util.PersistentMap;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Macros
{
  public sealed class T4MacroResolutionDataMarshaller : IUnsafeMarshaller<T4MacroResolutionData>
  {
    private static IUnsafeMarshaller<Dictionary<string, string>> StringDictionaryMarshaller { get; } =
      T4UnsafeMarshallers.GetDictionaryMarshaller(
        UnsafeMarshallers.UnicodeStringMarshaller,
        UnsafeMarshallers.UnicodeStringMarshaller
      );

    private T4MacroResolutionDataMarshaller()
    {
    }

    public static IUnsafeMarshaller<T4MacroResolutionData> Instance { get; } =
      new T4MacroResolutionDataMarshaller();

    public void Marshal(UnsafeWriter writer, [NotNull] T4MacroResolutionData value) =>
      StringDictionaryMarshaller.Marshal(
        writer,
        value.ResolvedMacros.ToDictionary(pair => pair.Key, pair => pair.Value)
      );

    [NotNull]
    public T4MacroResolutionData Unmarshal(UnsafeReader reader) =>
      new T4MacroResolutionData(StringDictionaryMarshaller.Unmarshal(reader));
  }
}