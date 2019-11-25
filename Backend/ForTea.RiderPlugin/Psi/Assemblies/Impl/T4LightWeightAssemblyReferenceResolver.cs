using System;
using System.IO;
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
			// If the argument is the fully qualified path of an existing file, then we are done.
			if (File.Exists(assemblyName)) return FileSystemPath.Parse(assemblyName);

			string folderPath = (file.ParentFolder?.Location?.FullPath).NotNull();

			try
			{
				// Maybe the assembly is in the same folder as the text template that called the directive?
				string candidate = Path.Combine(folderPath, assemblyName);
				if (File.Exists(candidate)) return FileSystemPath.Parse(candidate);
			}
			catch (ArgumentException)
			{
			}

			try
			{
				// Maybe the assembly name is missing extension?
				string candidate = Path.Combine(folderPath, assemblyName + ".dll");
				if (File.Exists(candidate)) return FileSystemPath.Parse(candidate);
			}
			catch (ArgumentException)
			{
			}

			return null;
		}
	}
}
