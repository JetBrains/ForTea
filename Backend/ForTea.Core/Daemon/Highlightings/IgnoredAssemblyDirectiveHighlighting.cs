using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	/// <summary>Error highlighting ignored assembly directives.</summary>
	[StaticSeverityHighlighting(
		Severity.WARNING,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.WARNING,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE
	)]
	public class IgnoredAssemblyDirectiveHighlighting : T4HighlightingBase<IT4Directive>
	{
		public override string ToolTip =>
			"Assembly directives are ignored in runtime templates. Use the assembly references of the project instead.";

		public IgnoredAssemblyDirectiveHighlighting([NotNull] IT4Directive directive) : base(directive)
		{
		}
	}
}
