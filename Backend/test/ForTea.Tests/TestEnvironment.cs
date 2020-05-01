using System.Threading;
using GammaJul.ForTea.Core;
using JetBrains.ForTea.RiderPlugin;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

namespace JetBrains.ForTea.Tests
{
	[SetUpFixture]
	public sealed class TestEnvironment : T4ExtensionTestEnvironmentAssembly<IT4TestZone> // HACK
	{
#pragma warning disable 169
		// These fields are here to force load assemblies
		private IT4Environment magic1; // ForTea.Core
		private T4RiderEnvironment magic2; // ForTea.RiderPlugin
#pragma warning restore 169
	}
}
