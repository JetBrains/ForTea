using System;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[CheckBuildProjectConfigurations]
internal class ForTeaBuild : NukeBuild
{
	[Parameter] public string Configuration;
	[Parameter] public string WaveVersion;
	[Parameter] public readonly string NuGetSource = "https://plugins.jetbrains.com/";
	[Parameter] public readonly string NuGetApiKey;
	[Solution] private readonly Solution Solution;
	private const string MainProjectName = "ForTea.ReSharperPlugin";
	private AbsolutePath OutputDirectory => RootDirectory / "artifacts" / Configuration;

	[NotNull]
	public Target InitializeConfiguration => target => target.Executes(() =>
	{
		if (Configuration != null)
		{
			Console.WriteLine($"Building for given configuration: {Configuration}");
			return;
		}
		// Configuration can be provided like this:
		// .\build.ps1 --configuration Release
		Console.WriteLine("Please, select configuration:");
		Console.WriteLine("  1: Release");
		Console.WriteLine("  2: Debug");
		Console.Write("Enter selection (default: Release) [1..2]:");
		string line = Console.ReadLine();
		if (string.IsNullOrEmpty(line))
		{
			Configuration = "Release";
			return;
		}

		int number = int.Parse(line);
		Configuration = number switch
		{
			1 => "Release",
			2 => "Debug",
			_ => throw new ArgumentOutOfRangeException()
		};
	});

	[NotNull]
	public Target InitializeWave => target => target.Executes(() =>
	{
		if (WaveVersion != null)
		{
			Console.WriteLine($"Building for given wave version: {WaveVersion}");
			return;
		}

		// Wave version can be provided like this:
		// .\build.ps1 --waveVersion 202.0
		Console.WriteLine("Please, enter wave version: ");
		WaveVersion = Console.ReadLine();
	});

	[NotNull]
	public Target Compile => target => target
		.DependsOn(InitializeConfiguration)
		.Executes(() => DotNetTasks.DotNetBuild(settings => settings
			.SetConfiguration(Configuration)
			.SetProjectFile(Solution)));

	[NotNull]
	public Target Pack => target => target
		.DependsOn(Compile, InitializeWave)
		.Executes(() => NuGetPack(settings => settings
			.SetTargetPath(RootDirectory / "ForTea.nuspec")
			.SetOutputDirectory(OutputDirectory)
			.SetProperty("jetBrainsYearSpan", GetJetBrainsYearSpan())
			.SetProperty("releaseNotes", GetLatestReleaseNotes())
			.SetProperty("configuration", Configuration)
			.SetProperty("wave", WaveVersion)
			.EnableNoPackageAnalysis()));

	[NotNull]
	public Target Push => target => target
		.DependsOn(Pack)
		.Requires(() => NuGetApiKey)
		.Requires(() => "Release".Equals(Configuration))
		.Requires(() => GlobFiles(OutputDirectory, "*.nupkg").Count == 1)
		.Executes(() => NuGetPush(settings => settings
			.SetTargetPath(GlobFiles(OutputDirectory, "*.nupkg").Single())
			.SetSource(NuGetSource)
			.SetApiKey(NuGetApiKey)));

	[NotNull]
	private string GetJetBrainsYearSpan()
	{
		const int startYear = 2019;
		int currentYear = DateTime.Now.Year;
		string startYearString = startYear.ToString(CultureInfo.InvariantCulture);
		if (currentYear == startYear) return startYearString;
		string currentYearString = currentYear.ToString(CultureInfo.InvariantCulture);
		return $"{startYearString}-{currentYearString}";
	}

	[NotNull]
	private static string GetLatestReleaseNotes() => File
		.ReadAllLines(RootDirectory.Parent.Parent / "CHANGELOG.md")
		.SkipWhile(x => !x.StartsWith("##", StringComparison.Ordinal))
		.Skip(1)
		.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
		.Select(x => $"\u2022{x.TrimStart('-')}")
		.JoinNewLine();

	public static int Main() => Execute<ForTeaBuild>(x => x.Compile);
}
