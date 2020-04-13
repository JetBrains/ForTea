using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
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

		public IProjectFile ResolutionContext { get; set; }

		[CanBeNull]
		private string RawPath =>
			this.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value?.GetText();

		public IT4PathWithMacros Path => CreateIncludePath(RawPath);

		[NotNull]
		public IT4PathWithMacros GetPathForParsing([NotNull] IPsiSourceFile file) =>
			new T4PathWithMacros(RawPath ?? "", file, ResolutionContext);

		[NotNull]
		private IT4PathWithMacros CreateIncludePath([CanBeNull] string includeFileName)
		{
			var sourceFile = this.GetParentOfType<IT4FileLikeNode>().NotNull().LogicalPsiSourceFile;
			return new T4PathWithMacros(includeFileName ?? "", sourceFile, ResolutionContext);
		}

		public IT4IncludedFile IncludedFile => NextSibling as IT4IncludedFile;
	}
}
