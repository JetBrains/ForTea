using System.Linq;

using GammaJul.ForTea.Core.Services;

using JetBrains.Application;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace JetBrains.ForTea.Tests.Mock
{
	[ShellComponent]
	public sealed class T4MockEnvironment : T4DefaultEnvironment
	{
		public override bool IsSupported => true;
	}
}