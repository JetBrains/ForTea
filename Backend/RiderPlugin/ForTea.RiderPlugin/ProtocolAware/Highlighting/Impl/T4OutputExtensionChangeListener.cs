using JetBrains.Annotations;
using JetBrains.Collections.Viewable;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Highlighting.Impl
{
  public sealed class T4OutputExtensionChangeListener
  {
    [NotNull] private IViewableProperty<string> Sink { get; }

    public T4OutputExtensionChangeListener([NotNull] IViewableProperty<string> sink) => Sink = sink;
    public void ExtensionChanged(string newExtension) => Sink.Value = newExtension;
  }
}