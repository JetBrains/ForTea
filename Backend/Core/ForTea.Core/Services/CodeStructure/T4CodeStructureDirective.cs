using System.Collections.Generic;
using System.IO;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.UI.Controls.TreeView;
using JetBrains.Application.UI.Controls.Utils;
using JetBrains.Application.UI.TreeModels;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Services.CodeStructure
{
  internal sealed class T4CodeStructureDirective : T4CodeStructureElement<IT4Directive>
  {
    [NotNull]
    private string GetDirectiveText()
    {
      IT4Directive directive = GetTreeNode();
      string name = directive?.Name?.GetText();
      if (name == null)
        return "???";

      DirectiveInfo directiveInfo = T4DirectiveInfoManager.GetDirectiveByName(name);
      if (directiveInfo == null)
        return name;

      // display the directive with the attributes that are marked with DisplayInCodeStructure
      var builder = new StringBuilder(name);
      foreach (IT4DirectiveAttribute attribute in directive.AttributesEnumerable)
      {
        DirectiveAttributeInfo attributeInfo = directiveInfo.GetAttributeByName(attribute.Name.GetText());
        if (attributeInfo?.IsDisplayedInCodeStructure != true)
          continue;

        builder.Append(' ');
        builder.Append(attributeInfo.Name);
        if (attribute.Value != null)
        {
          builder.Append("=\"");
          builder.Append(attribute.Value.GetText());
          builder.Append('"');
        }
      }

      return builder.ToString();
    }

    public override void Present(
      StructuredPresenter<TreeModelNode, IPresentableItem> presenter,
      IPresentableItem item,
      TreeModelNode modelNode,
      PresentationState state
    )
    {
      item.RichText = GetDirectiveText();
      item.Images.Add(PsiWebThemedIcons.AspDirective.Id);
    }

    protected override void DumpSelf(TextWriter builder)
      => builder.Write(GetDirectiveText());

    public override DocumentRange NavigationRange
    {
      get
      {
        IT4Directive directive = GetTreeNode();
        if (directive == null)
          return DocumentRange.InvalidRange;

        ITokenNode nameToken = directive.Name;
        if (nameToken == null)
          return directive.GetNavigationRange();

        return nameToken.GetNavigationRange();
      }
    }

    public override IList<string> GetQuickSearchTexts()
    {
      string name = GetTreeNode()?.Name.GetText();
      return name != null ? new[] { name } : EmptyList<string>.InstanceList;
    }

    public T4CodeStructureDirective(
      [NotNull] CodeStructureElement parent,
      [NotNull] IT4Directive directive
    ) : base(parent, directive)
    {
    }
  }
}