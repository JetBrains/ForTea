using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	internal partial class AssemblyDirective
	{
		public IProjectFile ResolutionContext { get; set; }

		public IT4PathWithMacros Path
		{
			get
			{
				string attributeName = T4DirectiveInfoManager.Assembly.NameAttribute.Name;
				string assemblyNameOrFile = this.GetAttributeValueByName(attributeName);
				return new T4PathWithMacros(assemblyNameOrFile ?? "", GetSourceFile().NotNull(), ResolutionContext);
			}
		}
	}
}
