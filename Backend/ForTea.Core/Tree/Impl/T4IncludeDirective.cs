using System;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	internal partial class T4IncludeDirective
	{
		public bool Once =>
			this
				.GetAttributes(T4DirectiveInfoManager.Include.OnceAttribute)
				.FirstOrDefault()
				?.Value
				.GetText()
				.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)
			?? false;

		public IT4PathWithMacros Path =>
			CreateIncludePath(this.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value.GetText());

		private IT4PathWithMacros CreateIncludePath([CanBeNull] string includeFileName)
		{
			if (includeFileName == null) return T4EmptyPathWithMacros.Instance;
			var sourceFile = GetSourceFile();
			if (sourceFile == null) return T4EmptyPathWithMacros.Instance;
			return new T4PathWithMacros(includeFileName, sourceFile);
		}
	}
}
