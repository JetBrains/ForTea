using System;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
  public static class T4CSharpCodeGenerationUtils
  {
    [NotNull] public const string DefaultTargetExtension = "cs";

    [NotNull]
    // name is NOT supposed to contain extension
    public static string WithExtension([NotNull] this string name, [NotNull] string extension)
    {
      if (name == null) throw new ArgumentNullException(nameof(name));
      if (extension == null) throw new ArgumentNullException(nameof(extension));
      return name + '.' + extension.WithoutLeadingDot();
    }

    [NotNull]
    public static string WithoutLeadingDot([NotNull] this string extension)
    {
      if (extension == null) throw new ArgumentNullException(nameof(extension));
      if (!extension.StartsWith(".", StringComparison.Ordinal)) return extension;
      return extension.Substring(1);
    }

    [NotNull]
    // name is supposed to contain extension
    public static string WithOtherExtension([NotNull] this string name, [NotNull] string newExtension)
    {
      if (name == null) throw new ArgumentNullException(nameof(name));
      if (newExtension == null) throw new ArgumentNullException(nameof(newExtension));
      return name.WithoutExtension().WithExtension(newExtension);
    }

    [NotNull]
    public static string WithoutExtension([NotNull] this string name)
    {
      if (name == null) throw new ArgumentNullException(nameof(name));
      int dotIndex = name.LastIndexOf('.');
      return dotIndex < 0 ? name : name.Substring(0, dotIndex);
    }

    [NotNull]
    public static string EscapeKeyword([NotNull] this string s)
    {
      if (!CSharpLexer.IsKeyword(s)) return s;
      return '@' + s;
    }

    /// <returns>
    /// Target extension. Leading dot, if any, is removed.
    /// Returns null if the file does not contain
    /// 'output' directive with 'extension' attribute.
    /// </returns>
    [CanBeNull]
    public static string GetTargetExtension([NotNull] this IT4File file)
    {
      if (file == null) throw new ArgumentNullException(nameof(file));

      var extension = T4DirectiveInfoManager.Output.ExtensionAttribute;
      string targetExtension = file
        .GetThisAndChildrenOfType<IT4OutputDirective>()
        .FirstOrDefault()
        ?.GetFirstAttribute(extension)
        ?.Value
        ?.GetText();

      if (targetExtension == null) return null;

      return targetExtension.StartsWith(".", StringComparison.Ordinal)
        ? targetExtension.Substring(1)
        : targetExtension;
    }
  }
}