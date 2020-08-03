using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Services;
using JetBrains.Application;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Microsoft.VisualStudio.TextTemplating;

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

				yield return typeof(ITextTemplatingEngineHost).Assembly.Location;
			}
		}

		public override IEnumerable<FileSystemPath> AdditionalCompilationAssemblyLocations
		{
			get { yield return FileSystemPath.Parse(typeof(Lifetime).Assembly.Location); }
		}

		public override bool IsSupported => true;
	}
}
