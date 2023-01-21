using System.Threading;
using GammaJul.ForTea.Core;
using JetBrains.ForTea.RiderPlugin;
using JetBrains.ForTea.TestsActivator;
using JetBrains.TestFramework;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

namespace JetBrains.ForTea.Tests
{
#if INDEPENDENT_BUILD
	[SetUpFixture]
	public sealed class TestEnvironment : T4ExtensionTestEnvironmentAssembly<T4TestsEnvZone> // HACK
	{
#pragma warning disable 169
		// These fields are here to force load assemblies
		private IT4Environment magic1; // ForTea.Core
		private T4RiderEnvironment magic2; // ForTea.RiderPlugin
#pragma warning restore 169
	}
#else
  [SetUpFixture]
  public sealed class TestEnvironment : ExtensionTestEnvironmentAssembly<T4TestsEnvZone>
  {
    public override bool IsRunningTestsWithAsyncBehaviorProhibited => true;
  }
#endif
}