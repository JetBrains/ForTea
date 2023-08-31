using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.BuildScript.PreCompile.Autofix;
using JetBrains.Application.BuildScript.Solution;
using JetBrains.Build;

namespace GammaJul.ForTea.BuildScript.Core
{
	public static class DefineForTeaConstants
	{
		[BuildStep]
		public static IEnumerable<AutofixAllowedDefineConstant> YieldAllowedDefineConstantsForMstest()
		{
			var constants = new List<string>();

			constants.AddRange(new[] {"INDEPENDENT_BUILD"});

			return constants.SelectMany(s => new []
			{
				new AutofixAllowedDefineConstant(new SubplatformName("Plugins\\ForTea\\Backend\\Core"), s),
				new AutofixAllowedDefineConstant(new SubplatformName("Plugins\\ForTea\\Backend\\RiderPlugin\\ForTea.RiderPlugin"), s),
				new AutofixAllowedDefineConstant(new SubplatformName("Plugins\\ForTea\\Backend\\RiderPlugin\\JetBrains.TextTemplating"), s),
				new AutofixAllowedDefineConstant(new SubplatformName("Plugins\\ForTea\\Backend\\RiderPlugin\\test"), s),
				new AutofixAllowedDefineConstant(new SubplatformName("Plugins\\ForTea\\Backend\\ReSharperPlugin"), s),
			});
		}
	}
}