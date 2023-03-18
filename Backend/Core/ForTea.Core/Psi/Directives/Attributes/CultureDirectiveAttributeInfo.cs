using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Directives.Attributes
{
  public class CultureDirectiveAttributeInfo : DirectiveAttributeInfo
  {
    [NotNull] [ItemNotNull] private readonly Lazy<JetHashSet<string>> _cultureCodes;
    [CanBeNull] [ItemNotNull] private ImmutableArray<string>? _intellisenseValues;

    [NotNull]
    private static JetHashSet<string> CreateCultureCodes()
    {
      var set = CultureInfo
        .GetCultures(CultureTypes.SpecificCultures)
        .ToJetHashSet(info => info.Name, StringComparer.OrdinalIgnoreCase);
      set.Add("");
      return set;
    }

    public override bool IsValid(string value)
      => _cultureCodes.Value.Contains(value);

    public override ImmutableArray<string> IntelliSenseValues
      => _intellisenseValues ?? (_intellisenseValues = _cultureCodes.Value.ToImmutableArray()).Value;

    public CultureDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
      : base(name, options)
    {
      _cultureCodes = Lazy.Of(CreateCultureCodes, true);
    }
  }
}