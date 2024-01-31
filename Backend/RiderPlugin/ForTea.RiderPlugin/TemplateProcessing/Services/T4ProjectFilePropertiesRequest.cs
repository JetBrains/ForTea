using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel.Properties;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services
{
  [ShellComponent(Instantiation.DemandAnyThreadSafe)]
  public class T4ProjectFilePropertiesRequest : IProjectFilePropertiesRequest
  {
    public const string AutoGenProperty = "AutoGen";
    public const string DesignTimeProperty = "DesignTime";
    public const string GeneratorProperty = "Generator";
    public const string LastGenOutputProperty = "LastGenOutput";

    private static IEnumerable<string> OurRequestedProperties { get; } = new[]
    {
      AutoGenProperty,
      DesignTimeProperty,
      GeneratorProperty,
      LastGenOutputProperty
    };

    public IEnumerable<string> RequestedProperties => OurRequestedProperties;
  }
}