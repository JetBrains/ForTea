using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Host.Features.RunMarkers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.RunMarkers
{
	[Language(typeof(T4Language))]
	public sealed class T4RunMarkerProvider : IRunMarkerProvider
	{
		public void CollectRunMarkers(
			[NotNull] IFile file,
			[NotNull] IContextBoundSettingsStore settings,
			[NotNull] IHighlightingConsumer consumer
		)
		{
			bool showMarkerOnStaticMethods = settings.GetValue((RunMarkerSettings s) => s.ShowMarkerOnStaticMethods);
			bool showMarkerOnEntryPoint = settings.GetValue((RunMarkerSettings s) => s.ShowMarkerOnEntryPoint);
			if (!showMarkerOnStaticMethods && !showMarkerOnEntryPoint) return;
			if (!(file is IT4File t4File)) return;
			if (t4File.PhysicalPsiSourceFile.ToProjectFile() == null) return;
			var directive = t4File.BlocksEnumerable.OfType<IT4TemplateDirective>().FirstOrDefault();
			if (directive == null) return;
			consumer.AddHighlighting(new T4RunMarkerHighlighting(directive));
		}

		public double Priority => RunMarkerProviderPriority.DEFAULT;
	}
}
