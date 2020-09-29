using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Feature.Services.UI;

namespace GammaJul.ForTea.Core.Daemon.Tooltip.Impl
{
	[SolutionComponent]
	public class T4EnvironmentVariableTooltipProvider :
		T4ExpandableElementTooltipProviderBase<IT4EnvironmentVariable>,
		IT4EnvironmentVariableTooltipProvider
	{
		public T4EnvironmentVariableTooltipProvider(
			Lifetime lifetime,
			ISolution solution,
			IDeclaredElementDescriptionPresenter presenter,
			[NotNull] DeclaredElementPresenterTextStylesService service
		) : base(lifetime, solution, presenter, service)
		{
		}

		protected override string Expand(IT4EnvironmentVariable variable)
		{
			var value = variable.RawAttributeValue;
			if (value == null) return null;
			return Environment.GetEnvironmentVariable(value.GetText());
		}
		protected override string ExpandableName => "environment variable";
	}
}
