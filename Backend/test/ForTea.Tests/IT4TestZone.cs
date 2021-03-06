using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Rider.Backend.Env;
using JetBrains.TestFramework.Application.Zones;

namespace JetBrains.ForTea.Tests
{
	[ZoneDefinition]
	public interface IT4TestZone : ITestsEnvZone,
		IRequire<PsiFeatureTestZone>,
		IRequire<IRiderPlatformZone>,
		IRequire<IPsiLanguageZone>,
		IRequire<ILanguageCSharpZone>
	{
	}
}
