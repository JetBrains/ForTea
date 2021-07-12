using System;
using GammaJul.ForTea.Core.Psi.Directives;

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

		protected override string RawPath =>
			this.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value?.GetText();

		public IT4IncludedFile IncludedFile => NextSibling as IT4IncludedFile;
	}
}
