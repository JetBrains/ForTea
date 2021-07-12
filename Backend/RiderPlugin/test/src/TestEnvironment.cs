using System.Threading;
using GammaJul.ForTea.Core;
using JetBrains.ForTea.RiderPlugin;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

namespace JetBrains.ForTea.Tests
{
	[SetUpFixture]
	public sealed class TestEnvironment : TestEnvironmentAssembly<IT4TestZone>
	{
		public override bool IsRunningTestsWithAsyncBehaviorProhibited
		{
			get { return true; }
		}

#pragma warning disable 169
		// These fields are here to force load assemblies
		// private IT4Environment magic1; // ForTea.Core
		// private T4RiderEnvironment magic2; // ForTea.RiderPlugin
#pragma warning restore 169
	}
}
