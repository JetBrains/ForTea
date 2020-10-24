using GammaJul.ForTea.Core.Daemon.Highlightings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Daemon.Tooltips.Request;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Tooltip
{
	[SolutionComponent]
	public sealed class T4MacroTooltipRequest : HighlightingBasedQuickDocTooltipRequest<MacroHighlighting>
	{
	}

	[SolutionComponent]
	public sealed class T4EnvironmentVariableTooltipRequest
		: HighlightingBasedQuickDocTooltipRequest<EnvironmentVariableHighlighting>
	{
	}
}
