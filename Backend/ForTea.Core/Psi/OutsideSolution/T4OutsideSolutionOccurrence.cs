using JetBrains.Annotations;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
	/// <summary>Represents an occurence of text in an include file that is located outside of the solution.</summary>
	internal sealed class T4OutsideSolutionOccurrence : IOccurrence
	{
		[NotNull]
		private IRangeMarker RangeMarker { get; }

		public bool IsValid => RangeMarker.IsValid;
		public OccurrenceType OccurrenceType => OccurrenceType.TextualOccurrence;
		public OccurrencePresentationOptions PresentationOptions { get; set; }
		public T4OutsideSolutionOccurrence([NotNull] IRangeMarker rangeMarker) => RangeMarker = rangeMarker;

		[NotNull]
		public string DumpToString() => RangeMarker.DocumentRange.ToString();

		public ISolution GetSolution() => null;

		public bool Navigate(
			ISolution solution,
			PopupWindowContextSource windowContext,
			bool transferFocus,
			TabOptions tabOptions = TabOptions.Default
		)
		{
			if (!IsValid) return false;
			var path = RangeMarker.Document.GetOutsideSolutionPath();
			if (path.IsEmpty) return false;
			var navigationInfo = new T4OutsideSolutionNavigationInfo(path, RangeMarker.DocumentRange);
			var navigationOptions = NavigationOptions.FromWindowContext(
				windowContext,
				"Navigate to included file",
				transferFocus,
				tabOptions
			);
			var navigationManager = NavigationManager.GetInstance(solution);
			return navigationManager
				.Navigate<T4OutsideSolutionNavigationProvider, T4OutsideSolutionNavigationInfo>(navigationInfo,
					navigationOptions);
		}
	}
}
