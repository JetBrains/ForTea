using System;
using GammaJul.ForTea.Core.Psi.Modules;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.DocumentManagers.PropertyModifiers;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Psi
{
  [SolutionFeaturePart(Instantiation.DemandAnyThreadUnsafe)]
  public class T4CSharpLanguageLevelProvider : CSharpLanguageLevelProvider
  {
    [NotNull] private readonly IT4Environment _t4Environment;

    public override bool IsApplicable(IPsiModule psiModule)
      => psiModule is IT4FilePsiModule;

    public override CSharpLanguageLevel GetLanguageLevel(IPsiModule psiModule)
      => _t4Environment.CSharpLanguageLevel;

    public override CSharpLanguageVersion? TryGetLanguageVersion(IPsiModule psiModule)
      => _t4Environment.CSharpLanguageLevel.ToLanguageVersion();

    public T4CSharpLanguageLevelProvider(
      [NotNull] IT4Environment t4Environment,
      [NotNull] CSharpLanguageLevelProjectProperty projectProperty,
      [CanBeNull] Lazy<ILanguageVersionModifier<CSharpLanguageVersion>> languageVersionModifier = null
    ) : base(projectProperty, languageVersionModifier) => _t4Environment = t4Environment;
  }
}