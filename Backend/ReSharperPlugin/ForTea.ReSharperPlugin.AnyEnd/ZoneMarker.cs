using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.RdFramework;
using JetBrains.Platform.VisualStudio.AnyEnd.BuildScript;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.ReSharperPlugin.AnyEnd;

[ZoneMarker]
public class ZoneMarker : IRequire<IVisualStudioProtocolEnvZone>, IRequire<IRdFrameworkZone>, IRequire<IProjectModelZone>;
