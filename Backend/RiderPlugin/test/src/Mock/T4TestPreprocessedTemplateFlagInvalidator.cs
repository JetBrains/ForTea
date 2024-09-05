using GammaJul.ForTea.Core.Psi.Cache.Impl;
using JetBrains.Application.Components;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.Tests.Mock
{
  // We don't need preprocessed template flag in backend tests
  [SolutionComponent(Instantiation.DemandAnyThreadSafe)]
  public sealed class
    T4TestPreprocessedTemplateFlagInvalidator : IHideImplementation<T4PreprocessedTemplateFlagInvalidator>
  {
  }
}