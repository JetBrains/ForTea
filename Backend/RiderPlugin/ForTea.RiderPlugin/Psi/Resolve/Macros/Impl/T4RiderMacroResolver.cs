using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros.Impl
{
  [SolutionComponent(InstantiationEx.LegacyDefault)]
  public class T4RiderMacroResolver : T4BasicMacroResolver
  {
    [NotNull] protected ISolution Solution { get; }

    [NotNull] protected IT4LightMacroResolver LightMacroResolver { get; }

    public T4RiderMacroResolver([NotNull] ISolution solution, [NotNull] IT4LightMacroResolver lightMacroResolver)
    {
      Solution = solution;
      LightMacroResolver = lightMacroResolver;
    }

    public override IReadOnlyDictionary<string, string> ResolveHeavyMacros(
      IEnumerable<string> macros,
      IProjectFile file
    ) => LightMacroResolver.ResolveAllLightMacros(file);

    private static ISet<string> UnsupportedMacros { get; } =
      new JetHashSet<string>(CaseInsensitiveComparison.Comparer)
      {
        "DevEnvDir",
        "FrameworkDir",
        "FrameworkSDKDir",
        "FrameworkVersion",
        "FxCopDir",
        "PlatformShortName",
        "ProjectExt",
        "RemoteMachine",
        "SolutionExt",
        "TargetFileName",
        "TargetName",
        "TargetPath",
        "VCInstallDir",
        "VSInstallDir",
        "WebDeployPath",
        "WebDeployRoot"
      };

    public override bool IsSupported(IT4Macro macro)
    {
      string value = macro.RawAttributeValue?.GetText();
      if (value == null) return true;
      return !UnsupportedMacros.Contains(value);
    }
  }
}