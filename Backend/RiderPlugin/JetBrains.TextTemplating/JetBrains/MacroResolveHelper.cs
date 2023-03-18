using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.TextTemplating.JetBrains
{
  public sealed class MacroResolveHelper
  {
    private static Regex MacroRegex { get; } =
      new Regex(@"\$\((\w+)\)",
        RegexOptions.Compiled |
        RegexOptions.IgnorePatternWhitespace);

    private IDictionary<string, string> KnownMacros { get; }

    public MacroResolveHelper(IDictionary<string, string> knownMacros) => KnownMacros = knownMacros;

    public string ExpandMacrosAndVariables(string s)
    {
      if (string.IsNullOrEmpty(s)) return s;
      return MacroRegex.Replace(Environment.ExpandEnvironmentVariables(s), match =>
      {
        var group = match.Groups[1];
        string macro = group.Value;
        if (!group.Success) return macro;
        if (!KnownMacros.TryGetValue(macro, out string value)) return macro;
        return value;
      });
    }
  }
}