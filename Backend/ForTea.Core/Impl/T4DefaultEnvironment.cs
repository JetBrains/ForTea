using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Impl
{
	[ShellComponent]
	public class T4DefaultEnvironment : IT4Environment
	{
		public virtual TargetFrameworkId TargetFrameworkId { get; } = TargetFrameworkId.Default;
		public virtual CSharpLanguageLevel CSharpLanguageLevel { get; } = CSharpLanguageLevel.Latest;

		public virtual IEnumerable<string> DefaultAssemblyNames
		{
			get { yield return "mscorlib"; }
		}

		public virtual bool IsSupported => false;
		public virtual IEnumerable<FileSystemPath> IncludePaths => EmptyList<FileSystemPath>.Enumerable;
	}
}
