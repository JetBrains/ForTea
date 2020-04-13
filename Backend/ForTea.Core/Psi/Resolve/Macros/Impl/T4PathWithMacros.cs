using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
	public sealed class T4PathWithMacros : IT4PathWithMacros
	{
		[NotNull]
		private static Regex MacroRegex { get; } =
			new Regex(@"\$\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private string RawPath { get; }

		public IPsiSourceFile SourceFile { get; }

		// Source file might have no corresponding project file;
		// This is the most suitable project file in that case
		public IProjectFile ProjectFile { get; }

		[NotNull]
		private IT4MacroResolver Resolver { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private ILogger Logger { get; } = JetBrains.Util.Logging.Logger.GetLogger<T4PathWithMacros>();

		public T4PathWithMacros(
			[CanBeNull] string rawPath,
			[NotNull] IPsiSourceFile file,
			[CanBeNull] IProjectFile projectFile,
			[CanBeNull] ISolution solution = null
		)
		{
			RawPath = rawPath ?? "";
			SourceFile = file;
			ProjectFile = projectFile;
			Solution = solution ?? SourceFile.GetSolution();
			ResolvedString = Lazy.Of(() =>
			{
				if (string.IsNullOrEmpty(RawPath) || !ContainsMacros) return RawPath;
				if (projectFile == null)
				{
					Logger.Warn("Could not find any project file for macro resolution");
					return RawPath;
				}

				var macroValues = Resolver.ResolveHeavyMacros(RawMacros, ProjectFile);
				string result = Environment.ExpandEnvironmentVariables(RawPath);
				return MacroRegex.Replace(result, match =>
				{
					var group = match.Groups[1];
					string macro = @group.Value;
					if (!@group.Success) return macro;
					if (!macroValues.TryGetValue(macro, out string value)) return macro;
					return value;
				});
			}, true);
			Resolver = Solution.GetComponent<IT4MacroResolver>();
		}

		public FileSystemPath TryResolveAbsolutePath()
		{
			string expanded = ResolveString();

			// search as absolute path
			var asAbsolutePath = FileSystemPath.TryParse(expanded);
			if (asAbsolutePath.IsAbsolute) return asAbsolutePath;

			// search as relative path
			var asRelativePath = SourceFile.GetLocation().Directory.TryCombine(expanded);
			if (asRelativePath.IsAbsolute && asRelativePath.ExistsFile) return asRelativePath;

			return null;
		}

		[NotNull, ItemNotNull]
		private Lazy<string> ResolvedString { get; }

		public string ResolveString() => ResolvedString.Value;

		private bool ContainsMacros
		{
			get
			{
				int lParen = RawPath.IndexOf("$(", StringComparison.Ordinal);
				int rParen = RawPath.IndexOf(")", StringComparison.Ordinal);
				return lParen >= 0 && rParen >= 0 && lParen <= rParen;
			}
		}

		[NotNull, ItemNotNull]
		private IEnumerable<string> RawMacros => MacroRegex
			.Matches(RawPath)
			.Cast<Match>()
			.Where(match => match.Success)
			.Select(match => match.Groups[1].Value);

		private bool Equals([NotNull] T4PathWithMacros other) =>
			string.Equals(RawPath, other.RawPath, StringComparison.OrdinalIgnoreCase)
			&& SourceFile.Equals(other.SourceFile);

		public override bool Equals(object obj) =>
			ReferenceEquals(this, obj) || obj is T4PathWithMacros other && Equals(other);

		public override int GetHashCode()
		{
			unchecked
			{
				return (RawPath.GetHashCode() * 397) ^ SourceFile.GetHashCode();
			}
		}
	}
}
