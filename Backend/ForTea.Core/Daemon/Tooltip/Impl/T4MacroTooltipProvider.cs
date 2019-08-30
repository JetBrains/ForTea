using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.Tooltip.Impl
{
	[SolutionComponent]
	public class T4MacroTooltipProvider : T4ExpandableElementTooltipProviderBase<IT4Macro>, IT4MacroTooltipProvider
	{
		[NotNull]
		private IT4MacroResolver Resolver { get; }

		public T4MacroTooltipProvider(
			Lifetime lifetime,
			ISolution solution,
			IDeclaredElementDescriptionPresenter presenter,
			[NotNull] IT4MacroResolver resolver
		) : base(lifetime, solution, presenter) => Resolver = resolver;

		protected override string Expand(IT4Macro macro)
		{
			var projectFile = macro.GetSourceFile().NotNull().ToProjectFile().NotNull();
			string name = macro.RawAttributeValue?.GetText();
			if (name == null) return null;
			var macros = Resolver.Resolve(new[] {name}, projectFile);
			return macros.ContainsKey(name) ? macros[name] : null;
		}

		protected override string ExpandableName => "macro";
	}
}
