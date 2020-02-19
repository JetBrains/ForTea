using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[CheckBuildProjectConfigurations]
internal class Build : NukeBuild
{
	[Parameter]
	public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Parameter] public readonly string NuGetSource = "https://plugins.jetbrains.com/";
	[Parameter] public readonly string WaveVersion;
	[Parameter] public readonly string NuGetApiKey;
	[Solution] private readonly Solution Solution;
	private const string MainProjectName = "ForTea.ReSharperPlugin";
	private AbsolutePath OutputDirectory => RootDirectory / "artifacts" / Configuration;

	[NotNull]
	public Target Compile => target => target
		.Executes(() => DotNetTasks.DotNetBuild(settings => settings
			.SetConfiguration(Configuration)
			.SetProjectFile(Solution)));

	[NotNull]
	public Target Pack => target => target
		.DependsOn(Compile)
		.Executes(() => NuGetPack(settings => settings
			.SetTargetPath(RootDirectory / "ForTea.nuspec")
			.SetOutputDirectory(OutputDirectory)
			.SetProperty("jetBrainsYearSpan", GetJetBrainsYearSpan())
			.SetProperty("releaseNotes", GetLatestReleaseNotes())
			.SetProperty("configuration", Configuration.ToString())
			.SetProperty("sdkVersion", GetReSharperSdkVersion())
			.SetProperty("wave", GetOrSelectWaveVersion())
			.EnableNoPackageAnalysis()));

	[NotNull]
	public Target Push => target => target
		.DependsOn(Pack)
		.Requires(() => NuGetApiKey)
		.Requires(() => Configuration.Release.Equals(Configuration))
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
		.ReadAllLines(RootDirectory.Parent / "CHANGELOG.md")
		.SkipWhile(x => !x.StartsWith("##", StringComparison.Ordinal))
		.Skip(1)
		.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
		.Select(x => $"\u2022{x.TrimStart('-')}")
		.JoinNewLine();

	[NotNull]
	private string GetOrSelectWaveVersion()
	{
		if (WaveVersion != null)
		{
			Console.WriteLine($"Building for given wave version: {WaveVersion}");
			return WaveVersion;
		}

		string selected = NuGetPackageResolver
			.GetLocalInstalledPackages(RootDirectory / MainProjectName / (MainProjectName + ".csproj"))
			.Where(x => x.Id == "Wave")
			.OrderByDescending(x => x.Version.Version)
			.FirstOrDefault()
			.NotNull("There's no R# installed and no wave version is given. Please, pass wave version as an argument: '--waveVersion 193.0'")
			.Version
			.Version
			.ToString(2);
		Console.WriteLine($"No WaveVersion is given. Auto-detected version:{selected}");
		return selected;
	}

	public static int Main() => Execute<Build>(x => x.Compile);
}
