using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GammaJul.ForTea.Core.Services;
using JetBrains.Application;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace JetBrains.ForTea.RiderPlugin
{
	[ShellComponent]
	public sealed class T4RiderEnvironment : T4DefaultEnvironment
	{
		public override TargetFrameworkId TargetFrameworkId =>
			TargetFrameworkId.AllKnownIds.Where(id => id.IsNetFramework).Max();

		public override CSharpLanguageLevel CSharpLanguageLevel => CSharpLanguageLevel.Latest;

		public override IEnumerable<string> DefaultAssemblyNames
		{
			get
			{
				foreach (string name in base.DefaultAssemblyNames)
				{
					yield return name;
				}

				yield return FileSystemPath.Parse(Assembly.GetExecutingAssembly().Location)
					.Parent
					.GetChildren("JetBrains.TextTemplating.dll")
					.Select(child => child.GetAbsolutePath())
					.Single()
					.FullPath;
			}
		}

		public override IEnumerable<FileSystemPath> AdditionalCompilationAssemblyLocations
		{
			get { yield return FileSystemPath.Parse(typeof(Lifetime).Assembly.Location); }
		}

		public override bool IsSupported => true;
	}
}
