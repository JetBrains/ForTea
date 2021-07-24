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

				string textTemplating = VirtualFileSystemPath.Parse(Assembly.GetExecutingAssembly().Location, InteractionContext.SolutionContext)
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

		public override IEnumerable<VirtualFileSystemPath> AdditionalCompilationAssemblyLocations
		{
			get { yield return VirtualFileSystemPath.Parse(typeof(Lifetime).Assembly.Location, InteractionContext.SolutionContext); }
		}

		public override bool IsSupported => true;
	}
}
