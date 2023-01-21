using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Daemon.Quickdocs
{
  public class T4EnvironmentQuickDocPresenter : T4QuickDocPresenterBase<IT4EnvironmentVariable>
  {
    public T4EnvironmentQuickDocPresenter([NotNull] IT4EnvironmentVariable expandable) : base(expandable)
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