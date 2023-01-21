using GammaJul.ForTea.Core.Psi.Directives;

namespace GammaJul.ForTea.Core.Tree.Impl
{
  internal partial class AssemblyDirective
  {
    protected override string RawPath =>
      this.GetAttributeValueByName(T4DirectiveInfoManager.Assembly.NameAttribute.Name);
  }
}