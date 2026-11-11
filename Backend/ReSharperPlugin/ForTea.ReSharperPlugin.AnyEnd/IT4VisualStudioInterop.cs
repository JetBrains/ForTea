using System.Collections.Generic;
using JetBrains.Application.Parts;
using JetBrains.Platform.VisualStudio.AnyEnd.Protocol.Env;
using JetBrains.ProjectModel.Search;
using JetBrains.Rd.Reflection;

namespace JetBrains.ForTea.ReSharperPlugin.AnyEnd;

[RdRpc]
[FrontendSolutionComponentInterface]
[DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
public interface IT4VisualStudioInterop
{
  Dictionary<string, string> ResolveMacros(IProjectModelElementSearchDescriptor project, string[] macros);

  Dictionary<string, string> ResolveAssemblies(
    IProjectModelElementSearchDescriptor project, string inputFile, string[] assemblyReferences);
}
