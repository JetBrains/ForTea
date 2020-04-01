using System;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Psi.Assemblies.Impl
{
	[SolutionComponent]
	public sealed class T4LightWeightAssemblyReferenceResolver : IT4LightWeightAssemblyReferenceResolver
	{
		public FileSystemPath TryResolve(IProjectFile file, string assemblyName)
		{
			try
			{
				// If the argument is the fully qualified path of an existing file, then we are done.
				var fullPath = FileSystemPath.TryParse(assemblyName);
				if (fullPath.IsAbsolute) return fullPath;
				var folderPath = (file.ParentFolder?.Location).NotNull();

				// Maybe the assembly is in the same folder as the text template that called the directive?
				var sameFolderPath = folderPath.Combine(assemblyName);
				if (sameFolderPath.ExistsFile) return sameFolderPath;

				// Maybe the assembly name is missing extension?
				var pathWithExtension =
					folderPath.Combine(assemblyName + ".dll");
				if (pathWithExtension.ExistsFile) return pathWithExtension;
			}
			catch (ArgumentException)
			{
				// If the assembly name contains illegal characters, we cannot resolve it (at least, this way)
			}

			return null;
		}
	}
}
