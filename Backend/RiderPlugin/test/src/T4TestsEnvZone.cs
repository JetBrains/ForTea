using GammaJul.ForTea.Core;

using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Rider.Backend.Env;
using JetBrains.TestFramework.Application.Zones;

// @formatter:wrap_extends_list_style chop_always
// @formatter:wrap_before_extends_colon true
// @formatter:wrap_before_comma true
// @formatter:alignment_tab_fill_style use_tabs_only
// @formatter:align_multiline_extends_list true

#pragma warning disable CheckNamespace

namespace JetBrains.ForTea.TestsActivator
{
	[ZoneDefinition]
	public class T4TestsEnvZone : ITestsEnvZone
	{
		[ZoneActivator]
		[ZoneMarker(typeof(T4TestsEnvZone))]
		public class T4TestsZoneActivator
			: IActivate<PsiFeatureTestZone>
			, IActivate<IT4Zone>
			, IActivate<IRiderPlatformZone>
			, IActivate<IPsiLanguageZone>
			, IActivate<ILanguageCSharpZone>
		{
		}
	}
}