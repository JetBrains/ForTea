using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders;
using GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[ProjectFileType(typeof(T4ProjectFileType))]
	public sealed class T4CSharpCustomFormattingInfoProvider : DummyCSharpCustomFormattingInfoProvider
	{
		private IReadOnlyList<IT4BlockSpaceTypeProvider> Providers { get; }

		public T4CSharpCustomFormattingInfoProvider() => Providers = new List<IT4BlockSpaceTypeProvider>
		{
			new T4ExpressionBlockOuterBoundSpaceTypeProvider(),
			new T4ExpressionBlockInnerBoundSpaceTypeProvider(),
			new T4FeatureBlockInnerBoundSpaceTypeProvider(),
			new T4InsideExpressionBlockSpaceTypeProvider()
		};

		public override FmtSettings<CSharpFormatSettingsKey> AdjustFormattingSettings(
			[NotNull] FmtSettings<CSharpFormatSettingsKey> settings,
			[NotNull] ISettingsOptimization settingsOptimization
		)
		{
			var cSharpFormatSettings = settings.Settings.Clone();
			cSharpFormatSettings.OLD_ENGINE = true;
			return settings.ChangeMainSettings(cSharpFormatSettings, true);
		}

		public override SpaceType GetBlockSpaceType(
			[NotNull] CSharpFmtStageContext ctx,
			[NotNull] CSharpCodeFormattingContext context
		) => Providers
			.Select(provider => provider.Provide(ctx))
			.Where(provided => provided.HasValue)
			.Select(provided => provided.Value)
			// default(SpaceType) == SpaceType.Default
			.FirstOrDefault();
	}
}
