using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	public abstract class T4DirectiveWithPathBase : T4CompositeElement, IT4DirectiveWithPath
	{
		[NotNull]
		private static Regex MacroRegex { get; } = new Regex(@"\$\((\w+)\)", RegexOptions.Compiled);

		public IEnumerable<string> RawMacros => MacroRegex
			.Matches(RawPath ?? "")
			.Cast<Match>()
			.Where(match => match.Success)
			.Select(match => match.Groups[1].Value);

		[CanBeNull]
		private T4ResolvedPath MyResolvedPath { get; set; }

		public T4ResolvedPath ResolvedPath => MyResolvedPath.NotNull();

		public void InitializeResolvedPath(
			IReadOnlyDictionary<string, string> resolveResults,
			IPsiSourceFile sourceFile,
			IProjectFile projectFile
		)
		{
			string expanded = Environment.ExpandEnvironmentVariables(RawPath ?? "");
			string result = MacroRegex.Replace(expanded, match =>
			{
				var group = match.Groups[1];
				string macro = group.Value;
				if (!group.Success) return macro;
				if (!resolveResults.TryGetValue(macro, out string value)) return macro;
				return value;
			});
			MyResolvedPath = new T4ResolvedPath(result, sourceFile, projectFile);
		}

		[CanBeNull]
		protected abstract string RawPath { get; }
	}
}
