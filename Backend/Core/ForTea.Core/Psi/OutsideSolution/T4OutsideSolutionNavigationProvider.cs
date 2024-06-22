using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationProviders;
using JetBrains.ReSharper.Features.Navigation.Core.Navigation;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
  [NavigationProvider(Instantiation.DemandAnyThreadUnsafe)]
  public sealed class T4OutsideSolutionNavigationProvider : INavigationProvider<T4OutsideSolutionNavigationInfo>
  {
    [NotNull] private FileSystemPathNavigator Navigator { get; }

    public T4OutsideSolutionNavigationProvider([NotNull] FileSystemPathNavigator fileSystemPathNavigator) =>
      Navigator = fileSystemPathNavigator;

    public bool IsApplicable([CanBeNull] T4OutsideSolutionNavigationInfo data) => data != null;

    [NotNull, ItemNotNull]
    public IEnumerable<INavigationPoint> CreateNavigationPoints(
      [NotNull] T4OutsideSolutionNavigationInfo target
    ) => new INavigationPoint[]
    {
      Navigator.CreateNavigationPoint(target.FileSystemPath, target.DocumentRange.TextRange, "T4", "T4")
    };
  }
}