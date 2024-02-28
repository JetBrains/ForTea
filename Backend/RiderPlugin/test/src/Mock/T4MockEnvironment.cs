using System.Linq;
using GammaJul.ForTea.Core.Services;
using JetBrains.Application;
using JetBrains.Application.Parts;

namespace JetBrains.ForTea.Tests.Mock
{
  [ShellComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class T4MockEnvironment : T4DefaultEnvironment
  {
    public override bool IsSupported => true;
  }
}