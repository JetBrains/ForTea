using GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Psi.Assemblies.Impl
{
	[SolutionComponent]
	public sealed class T4LightWeightAssemblyReferenceResolver : T4BasicLightWeightAssemblyReferenceResolver
	{
		public override FileSystemPath TryResolve(IT4PathWithMacros path)
		{
			var absolutePath = path.TryResolveAbsolutePath();
			if (absolutePath != null) return absolutePath;
			// Maybe the assembly name is missing extension?
			var pathWithExtension = path.ProjectFile.ParentFolder?.Location.TryCombine(path.ResolveString() + ".dll");
			if (pathWithExtension == null) return null;
			if (pathWithExtension.ExistsFile) return pathWithExtension;
			return null;
		}
	}
}
