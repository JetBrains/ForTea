using System.Linq;
using GammaJul.ForTea.Core.Services;
using JetBrains.Application;

namespace JetBrains.ForTea.Tests.Mock
{
  [ShellComponent]
  public sealed class T4MockEnvironment : T4DefaultEnvironment
  {
    public override bool IsSupported => true;
  }
}