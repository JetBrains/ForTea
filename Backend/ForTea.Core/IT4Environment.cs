using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core
{
	public interface IT4Environment
	{
		/// <summary>Gets the target framework ID.</summary>
		[NotNull]
		TargetFrameworkId TargetFrameworkId { get; }

		/// <summary>Gets the C# language version.</summary>
		CSharpLanguageLevel CSharpLanguageLevel { get; }

		/// <summary>Gets the default included assemblies.</summary>
		[NotNull, ItemNotNull]
		IEnumerable<string> DefaultAssemblyNames { get; }

		[NotNull, ItemNotNull]
		IEnumerable<FileSystemPath> AdditionalCompilationAssemblyLocations { get; }

		/// <summary>Gets whether the current environment is supported. VS2005 and VS2008 aren't.</summary>
		bool IsSupported { get; }

		/// <summary>Gets the common include paths from the registry.</summary>
		[NotNull, ItemNotNull]
		IEnumerable<FileSystemPath> IncludePaths { get; }
	}
}
