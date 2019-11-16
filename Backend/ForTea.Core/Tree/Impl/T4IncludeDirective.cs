using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	internal partial class IncludeDirective
	{
		public bool Once =>
			this.GetFirstAttribute(T4DirectiveInfoManager.Include.OnceAttribute)
				?.Value
				.GetText()
				.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)
			?? false;

		[CanBeNull]
		private string RawPath =>
			this.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value?.GetText();

		public IT4PathWithMacros Path => CreateIncludePath(RawPath);

		[NotNull]
		public IT4PathWithMacros GetPathForParsing([NotNull] IPsiSourceFile file)
		{
			string rawPath = RawPath;
			if (rawPath == null) return T4EmptyPathWithMacros.Instance;
			return new T4PathWithMacros(rawPath, file);
		}

		private IT4PathWithMacros CreateIncludePath([CanBeNull] string includeFileName)
		{
			if (includeFileName == null) return T4EmptyPathWithMacros.Instance;
			var sourceFile = GetSourceFile();
			if (sourceFile == null) return T4EmptyPathWithMacros.Instance;
			return new T4PathWithMacros(includeFileName, sourceFile);
		}

		public IT4IncludedFile IncludedFile => NextSibling as IT4IncludedFile;
	}
}
