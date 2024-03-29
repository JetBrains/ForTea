using GammaJul.ForTea.Core;
using JetBrains.Application.Components;
using JetBrains.Application.StdApplicationUI.TaskBar;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.General
{
  [TestFixture]
  public sealed class T4EnvironmentTest : BaseTest
  {
    [Test]
    public void TestThatT4EnvironmentSupportsEverything()
    {
      var environment = ShellInstance.TryGetComponent<IT4Environment>();
      Assert.NotNull(environment);
      Assert.That(environment.IsSupported);
    }

    [Test]
    public void TestThatSomePlatformShellComponentIsAccessible() =>
      Assert.NotNull(ShellInstance.GetComponent<ITaskBarManager>());
  }
}