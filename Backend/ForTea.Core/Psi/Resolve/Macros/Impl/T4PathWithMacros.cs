using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
	public sealed class T4PathWithMacros : IT4PathWithMacros
	{
		[NotNull]
		private static Regex MacroRegex { get; } =
			new Regex(@"\$\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		[NotNull]
		private string RawPath { get; }

		[NotNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private IT4MacroResolver Resolver { get; }

		[NotNull]
		private IT4Environment Environment { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private ILogger Logger { get; } = JetBrains.Util.Logging.Logger.GetLogger<T4PathWithMacros>();

		[NotNull]
		private T4OutsideSolutionSourceFileManager OutsideSolutionManager { get; }

		public T4PathWithMacros([CanBeNull] string rawPath, [NotNull] IPsiSourceFile file)
		{
			RawPath = rawPath ?? "";
			SourceFile = file;
			Solution = SourceFile.GetSolution();
			Resolver = Solution.GetComponent<IT4MacroResolver>();
			Environment = Solution.GetComponent<IT4Environment>();
			OutsideSolutionManager = Solution.GetComponent<T4OutsideSolutionSourceFileManager>();
		}

		public IT4File ResolveT4File(T4IncludeGuard guard)
		{
			if (!ResolvePath().ExistsFile) return null;
			var target = Resolve();
			if (target == null) return null;
			if (!guard.CanProcess(target)) return null;
			if (target.LanguageType.Is<T4ProjectFileType>())
				return (IT4File) target.GetPrimaryPsiFile();
			return BuildT4Tree(target);
		}

		[NotNull]
		private IT4File BuildT4Tree([NotNull] IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService().NotNull();
			var lexer = (T4Lexer) languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			var file = new T4Parser(lexer).Parse();
			file.SetSourceFile(target);
			return file;
		}

		public IPsiSourceFile Resolve()
		{
			var path = ResolvePath();
			if (path.IsEmpty) return null;
			var solutionSourceFile = Solution
				.FindProjectItemsByLocation(path)
				.OfType<IProjectFile>()
				.SingleItem()?.ToSourceFile();
			var file = solutionSourceFile;
			if (file != null) return file;
			if (path.ExistsFile) return OutsideSolutionManager.GetOrCreateSourceFile(path);
			return null;
		}

		public FileSystemPath ResolvePath()
		{
			string expanded = ResolveString();

			// search as absolute path
			var asAbsolutePath = FileSystemPath.TryParse(expanded);
			if (asAbsolutePath.IsAbsolute) return asAbsolutePath;

			// search as relative path
			var asRelativePath = SourceFile.GetLocation().Directory.TryCombine(expanded);
			if (asRelativePath.ExistsFile) return asRelativePath;

			// search in global include paths
			var asGlobalInclude = Environment.IncludePaths
				.Select(includePath => includePath.Combine(expanded))
				.FirstOrDefault(resultPath => resultPath.ExistsFile);

			return asGlobalInclude ?? asRelativePath;
		}

		public string ResolveString()
		{
			if (string.IsNullOrEmpty(RawPath) || !ContainsMacros) return RawPath;
			var projectFile = SourceFile.ToProjectFile() ?? T4MacroResolveContextCookie.ProjectFile;
			if (projectFile == null)
			{
				Logger.Warn("Could not find any project file for macro resolution");
				return RawPath;
			}
			var macroValues = Resolver.ResolveHeavyMacros(RawMacros, projectFile);
			string result = System.Environment.ExpandEnvironmentVariables(RawPath);
			return MacroRegex.Replace(result, match =>
			{
				var group = match.Groups[1];
				string macro = group.Value;
				if (!group.Success) return macro;
				if (!macroValues.TryGetValue(macro, out string value)) return macro;
				return value;
			});
		}

		private bool ContainsMacros
		{
			get
			{
				int lParen = RawPath.IndexOf("$(", StringComparison.Ordinal);
				int rParen = RawPath.IndexOf(")", StringComparison.Ordinal);
				return lParen >= 0 && rParen >= 0 && lParen <= rParen;
			}
		}

		private IEnumerable<string> RawMacros => MacroRegex
			.Matches(RawPath)
			.Cast<Match>()
			.Where(match => match.Success)
			.Select(match => match.Groups[1].Value);

		private bool Equals(T4PathWithMacros other) =>
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

		public bool IsEmpty => RawPath.IsEmpty();
	}
}
