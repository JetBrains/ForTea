using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
  [Language(typeof(T4Language))]
  public sealed class T4CodeFoldingProcessorFactory : ICodeFoldingProcessorFactory
  {
    [NotNull]
    public ICodeFoldingProcessor CreateProcessor(IContextBoundSettingsStore settingsStore) => new T4CodeFoldingProcessor();
  }
}