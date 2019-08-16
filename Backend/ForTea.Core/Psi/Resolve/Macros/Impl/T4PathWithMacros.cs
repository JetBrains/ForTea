using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Modules;
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

		private string RawPath { get; }

		[NotNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private IT4MacroResolver Resolver { get; }

		[NotNull]
		private IT4Environment Environment { get; }

		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private ISolution Solution { get; }

		[CanBeNull]
		private IT4FilePsiModule Module => SourceFile.PsiModule as IT4FilePsiModule;

		public T4PathWithMacros([NotNull] string rawPath, [NotNull] IPsiSourceFile file)
		{
			RawPath = rawPath;
			SourceFile = file;
			Solution = SourceFile.GetSolution();
			Resolver = Solution.GetComponent<IT4MacroResolver>();
			Environment = Solution.GetComponent<IT4Environment>();
			Manager = Solution.GetComponent<T4DirectiveInfoManager>();
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

		private IT4File BuildT4Tree(IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService();
			Assertion.AssertNotNull(languageService, "languageService != null");
			var lexer = languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			return new T4TreeBuilder(Manager, lexer, target).CreateT4Tree();
		}

		public IPsiSourceFile Resolve() => ResolvePath().FindSourceFileInSolution(Solution);

		public FileSystemPath ResolvePath()
		{
			string expanded = ResolveString();

			// search as absolute path
			var asAbsolutePath = FileSystemPath.TryParse(expanded);
			if (asAbsolutePath.IsAbsolute) return asAbsolutePath;

			// search as relative path
			var asRelativePath = SourceFile.GetLocation().Directory.Combine(expanded);
			if (asRelativePath.ExistsFile) return asRelativePath;

			// search in global include paths
			var asGlobalInclude = Environment.IncludePaths
				.Select(includePath => includePath.Combine(expanded))
				.FirstOrDefault(resultPath => resultPath.ExistsFile);

			return asGlobalInclude ?? asRelativePath;
		}

		public string ResolveString()
		{
			var module = Module;
			if (string.IsNullOrEmpty(RawPath) || module == null || !ContainsMacros) return RawPath;

			var macroValues = Resolver.Resolve(RawMacros, SourceFile.ToProjectFile().NotNull());

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
				return ((RawPath != null ? RawPath.GetHashCode() : 0) * 397) ^ SourceFile.GetHashCode();
			}
		}

		public bool IsEmpty => RawPath.IsEmpty();
	}
}
