using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GammaJul.ForTea.Core.Services;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.platforms;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.DevEnv;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Microsoft.Win32;

namespace JetBrains.ForTea.ReSharperPlugin
{
  /// <summary>Contains environment-dependent information.</summary>
  [ShellComponent]
  public sealed class T4ReSharperEnvironment : T4DefaultEnvironment
  {
    // temporary solution until that constant is added into the SDK
    private const int VsVersion2022 = 17;
    [NotNull] private readonly IVsEnvironmentStaticInformation _vsEnvironmentInformation;
    [NotNull] private readonly string[] _textTemplatingAssemblyNames;
    [CanBeNull] private readonly TargetFrameworkId _targetFrameworkId;
    [CanBeNull] private IList<VirtualFileSystemPath> _includePaths;

    /// <summary>Gets the target framework ID.</summary>
    public override TargetFrameworkId TargetFrameworkId
    {
      get
      {
        if (_targetFrameworkId == null) throw Unsupported();
        return _targetFrameworkId;
      }
    }

    /// <summary>Gets the C# language version.</summary>
    public override CSharpLanguageLevel CSharpLanguageLevel { get; }

    /// <summary>Gets the default included assemblies.</summary>
    [NotNull]
    private IEnumerable<string> TextTemplatingAssemblyNames
    {
      get
      {
        if (_targetFrameworkId == null)
          throw Unsupported();
        return _textTemplatingAssemblyNames;
      }
    }

    public override IEnumerable<string> DefaultAssemblyNames =>
      TextTemplatingAssemblyNames.Concat(base.DefaultAssemblyNames);

    /// <summary>Gets whether the current environment is supported. VS2005 and VS2008 aren't.</summary>
    public override bool IsSupported => _targetFrameworkId != null;

    /// <summary>Gets the common include paths from the registry.</summary>
    public override IEnumerable<VirtualFileSystemPath> IncludePaths
    {
      get
      {
        if (_targetFrameworkId == null)
          return EmptyList<VirtualFileSystemPath>.InstanceList;
        return _includePaths ?? (_includePaths = ReadIncludePaths());
      }
    }

    [NotNull]
    private IList<VirtualFileSystemPath> ReadIncludePaths()
    {
      string registryKey = _vsEnvironmentInformation.VisualStudioGlobalRegistryPath
                           + @"_Config\TextTemplating\IncludeFolders\.tt";

      using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
      {
        if (key == null)
          return EmptyList<VirtualFileSystemPath>.InstanceList;

        string[] valueNames = key.GetValueNames();
        if (valueNames.Length == 0)
          return EmptyList<VirtualFileSystemPath>.InstanceList;

        var paths = new List<VirtualFileSystemPath>(valueNames.Length);
        foreach (string valueName in valueNames)
        {
          var value = key.GetValue(valueName) as string;
          if (String.IsNullOrEmpty(value))
            continue;

          var path = VirtualFileSystemPath.TryParse(value, InteractionContext.SolutionContext);
          if (!path.IsEmpty && path.IsAbsolute)
            paths.Add(path);
        }

        return paths;
      }
    }

    [NotNull, Pure]
    private static NotSupportedException Unsupported() => new("Unsupported environment");

    [NotNull]
    private static string CreateGacAssemblyName([NotNull] string name, int majorVersion)
      => String.Format(
        CultureInfo.InvariantCulture,
        "{0}.{1}.0, Version={1}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        name,
        majorVersion);

    [NotNull]
    [Pure]
    private static string CreateDevEnvPublicAssemblyName(
      [NotNull] IVsEnvironmentStaticInformation vsEnvironmentInformation, [NotNull] string name)
      => vsEnvironmentInformation
        .DevEnvInstallDir
        .Combine(RelativePath.Parse("PublicAssemblies\\" + name + ".dll"))
        .FullPath;

    public T4ReSharperEnvironment([NotNull] IVsEnvironmentStaticInformation vsEnvironmentInformation)
    {
      _vsEnvironmentInformation = vsEnvironmentInformation;

      switch (vsEnvironmentInformation.VsVersion2.Major)
      {
        case VsVersions.Vs2010:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 0));
          CSharpLanguageLevel = CSharpLanguageLevel.CSharp40;
          _textTemplatingAssemblyNames = new[]
          {
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 10),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
          };
          break;

        case VsVersions.Vs2012:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 5));
          CSharpLanguageLevel = CSharpLanguageLevel.CSharp50;
          _textTemplatingAssemblyNames = new[]
          {
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 11),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 11),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
          };
          break;

        case VsVersions.Vs2013:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 5));
          CSharpLanguageLevel = CSharpLanguageLevel.CSharp50;
          _textTemplatingAssemblyNames = new[]
          {
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 12),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 11),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
          };
          break;

        case VsVersions.Vs2015:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 5));
          const int vs2015Update2Build = 25123;
          CSharpLanguageLevel = vsEnvironmentInformation.VsVersion4.Build >= vs2015Update2Build
            ? CSharpLanguageLevel.CSharp60
            : CSharpLanguageLevel.CSharp50;
          _textTemplatingAssemblyNames = new[]
          {
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 14),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 11),
            CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
          };
          break;

        case VsVersions.Vs2017:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 6));
          CSharpLanguageLevel = CSharpLanguageLevel.CSharp70;
          _textTemplatingAssemblyNames = new[]
          {
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation, "Microsoft.VisualStudio.TextTemplating.15.0"),
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation,
              "Microsoft.VisualStudio.TextTemplating.Interfaces.11.0"),
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation,
              "Microsoft.VisualStudio.TextTemplating.Interfaces.10.0")
          };
          break;

        case VsVersions.Vs2019:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 7, 2));
          CSharpLanguageLevel = CSharpLanguageLevel.CSharp73;
          _textTemplatingAssemblyNames = new[]
          {
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation, "Microsoft.VisualStudio.TextTemplating.15.0"),
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation,
              "Microsoft.VisualStudio.TextTemplating.Interfaces.11.0"),
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation,
              "Microsoft.VisualStudio.TextTemplating.Interfaces.10.0")
          };
          break;

        case VsVersion2022:
          _targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 8));
          CSharpLanguageLevel = CSharpLanguageLevel.CSharp100;
          _textTemplatingAssemblyNames = new[]
          {
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation, "Microsoft.VisualStudio.TextTemplating"),
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation,
              "Microsoft.VisualStudio.TextTemplating.Interfaces.11.0"),
            CreateDevEnvPublicAssemblyName(vsEnvironmentInformation,
              "Microsoft.VisualStudio.TextTemplating.Interfaces.10.0")
          };
          break;

        default:
          _textTemplatingAssemblyNames = EmptyArray<string>.Instance;
          break;
      }
    }
  }
}