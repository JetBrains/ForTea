using System;
using System.Globalization;

namespace Microsoft.VisualStudio.TextTemplating
{
  /// <summary>
  ///     Utility class to produce culture-oriented representation of an object as a string.
  /// </summary>
  public static class ToStringHelper
  {
    private static IFormatProvider formatProviderField =
      CultureInfo.InvariantCulture;

    /// <summary>
    ///     Gets or sets format provider to be used by ToStringWithCulture method.
    /// </summary>
    public static IFormatProvider FormatProvider
    {
      get => formatProviderField;
      set
      {
        if (value == null) return;
        formatProviderField = value;
      }
    }

    /// <summary>
    ///     This is called from the compile/run appdomain to convert objects within an expression block to a string
    /// </summary>
    public static string ToStringWithCulture(object objectToConvert)
    {
      if (objectToConvert == null) throw new ArgumentNullException(nameof(objectToConvert));

      var t = objectToConvert.GetType();
      var method = t.GetMethod("ToString", new[]
      {
        typeof(IFormatProvider)
      });
      if (method == null)
        return objectToConvert.ToString();
      return (string)method.Invoke(objectToConvert, new object[] { formatProviderField });
    }
  }
}