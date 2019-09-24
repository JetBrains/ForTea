using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.platforms;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace JetBrains.ForTea.RiderPlugin
{
	// TODO: use more accurate values
	[ShellComponent]
	public sealed class T4Environment : IT4Environment
	{
		public TargetFrameworkId TargetFrameworkId { get; } =
			TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 7, 2));

		public CSharpLanguageLevel CSharpLanguageLevel => CSharpLanguageLevel.Latest;

		public IEnumerable<string> TextTemplatingAssemblyNames
		{
			get
			{
				var assembliesDirectory = BestTextTemplatingDirectory / "Common7" / "IDE" / "PublicAssemblies";
				var entries = assembliesDirectory.GetDirectoryEntries("Microsoft.VisualStudio.TextTemplating.*");
				return entries.Select(it => it.GetAbsolutePath().FullPath);
			}
		}

		[NotNull]
		private static FileSystemPath BestTextTemplatingDirectory
		{
			get
			{
				var baseDirectory = FileSystemPath.TryParse(@"C:\Program Files (x86)\Microsoft Visual Studio");
				if (baseDirectory.IsEmpty || !baseDirectory.ExistsDirectory) return FileSystemPath.Empty;
				var result = new[] {"2021", "2019", "2017", "2015", "2013", "2012", "2010"}
					.Select(version => baseDirectory / version)
					.SelectMany(candidate => new[] {candidate / "Community", candidate / "Professional"})
					.FirstOrDefault(candidate => candidate.ExistsDirectory);
				return result ?? FileSystemPath.Empty;
			}
		}

		public bool IsSupported => true;
		public IEnumerable<FileSystemPath> IncludePaths => Enumerable.Empty<FileSystemPath>();
	}
}
