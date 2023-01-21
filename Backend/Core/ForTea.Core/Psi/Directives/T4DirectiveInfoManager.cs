using System;
using System.Collections.Immutable;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.VB;

namespace GammaJul.ForTea.Core.Psi.Directives
{
  public static class T4DirectiveInfoManager
  {
    /// <summary>Gets information about the template directive.</summary>
    [NotNull]
    public static TemplateDirectiveInfo Template { get; }

    /// <summary>Gets information about the parameter directive.</summary>
    [NotNull]
    public static ParameterDirectiveInfo Parameter { get; }

    /// <summary>Gets information about the output directive.</summary>
    [NotNull]
    public static OutputDirectiveInfo Output { get; }

    /// <summary>Gets information about the include directive.</summary>
    [NotNull]
    public static IncludeDirectiveInfo Include { get; }

    /// <summary>Gets information about the assembly directive.</summary>
    [NotNull]
    public static AssemblyDirectiveInfo Assembly { get; }

    /// <summary>Gets information about the import directive.</summary>
    [NotNull]
    public static ImportDirectiveInfo Import { get; }

    /// <summary>Gets a collection of all known directives.</summary>
    /// <remarks>Order of elements in this collection will be used to order the directives.</remarks>
    [NotNull, ItemNotNull]
    public static ImmutableArray<DirectiveInfo> AllDirectives { get; }

    [CanBeNull]
    public static DirectiveInfo GetDirectiveByName([CanBeNull] string directiveName) =>
      string.IsNullOrEmpty(directiveName)
        ? null
        : AllDirectives.FirstOrDefault(di => di.Name.Equals(directiveName, StringComparison.OrdinalIgnoreCase));

    public static PsiLanguageType GetLanguageType([CanBeNull] IT4File file)
    {
      string name = file
        ?.Blocks
        .OfType<IT4TemplateDirective>()
        .FirstOrDefault()
        ?.GetFirstAttribute(Template.LanguageAttribute)
        ?.Value
        ?.GetText();
      switch (name)
      {
        case null:
        case LanguageAttributeInfo.CSharpLanguageAttributeValue:
        case LanguageAttributeInfo.NewCSharpLanguageAttributeValue:
          return CSharpLanguage.Instance;
        case LanguageAttributeInfo.VBLanguageAttributeValue:
          return VBLanguage.Instance;
        default:
          return UnknownLanguage.Instance;
      }
    }

    static T4DirectiveInfoManager()
    {
      Template = new TemplateDirectiveInfo();
      Parameter = new ParameterDirectiveInfo();
      Output = new OutputDirectiveInfo();
      Include = new IncludeDirectiveInfo();
      Assembly = new AssemblyDirectiveInfo();
      Import = new ImportDirectiveInfo();
      AllDirectives = ImmutableArray.Create<DirectiveInfo>(
        Template,
        Parameter,
        Output,
        Include,
        Assembly,
        Import
      );
    }
  }
}