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

				string textTemplating = FileSystemPath.Parse(Assembly.GetExecutingAssembly().Location)
					.Parent
					.GetChildren("JetBrains.TextTemplating.dll")
					.Select(child => child.GetAbsolutePath())
					.SingleItem()
					?.FullPath;
				if (textTemplating != null)
				{
					yield return textTemplating;
				}
			}
		}

		public override IEnumerable<FileSystemPath> AdditionalCompilationAssemblyLocations
		{
			get
			{
				var lifetimesLocation = typeof(Lifetime).Assembly.Location;
				var lifetimesPath = FileSystemPath.Parse(lifetimesLocation);
				var classicLifetimesPath = lifetimesPath.Parent.Parent.GetChildren(lifetimesPath.Name);
				if (!classicLifetimesPath.IsEmpty())
				{
					// If we are running on dotnet core, this is the location of the classical dll
					yield return classicLifetimesPath.First().GetAbsolutePath();
				}
				else
				{
					yield return lifetimesPath;
				}
			}
		}

		public override bool IsSupported => true;
	}
}
