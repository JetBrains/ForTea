using JetBrains.Annotations;
using JetBrains.EnvDTE.Host;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
  public readonly struct T4EnvDTEHost
  {
    [NotNull] public LifetimeDefinition LifetimeDefinition { get; }

    [NotNull] public ConnectionManager ConnectionManager { get; }

    public T4EnvDTEHost(
      [NotNull] LifetimeDefinition lifetimeDefinition,
      ISolution solution
    )
    {
      LifetimeDefinition = lifetimeDefinition;
      ConnectionManager = new ConnectionManager(lifetimeDefinition.Lifetime, solution);
    }
  }
}